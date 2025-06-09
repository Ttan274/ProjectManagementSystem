using AutoMapper;
using Octokit;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto;
using ProjectManagementSystem.Common.ServiceResponse;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace ProjectManagementSystem.Application.GitHubRepoAnalytics
{
    public class GitHubRepoAnalyticsService(
        IServiceResponseHelper serviceResponseHelper,
        IMapper mapper)
        : IGitHubRepoAnalyticsService
    {
        private static GitHubClient CreateGitHubClient(string? accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentNullException(nameof(accessToken));
            }

            return new GitHubClient(new ProductHeaderValue("ProjectManAnalyzer"))
            {
                Credentials = new Credentials(accessToken)
            };
        }

        public async Task<ServiceResponse<List<UserCommitStatsDto>>> GetCommitCountsByUserAsync(AppGitCredentialDto appGitCredential)
        {
            try
            {
                var gitHubClient = CreateGitHubClient(appGitCredential.PatToken);
                var userStatsDict = new Dictionary<string, UserCommitStatsDto>();

                var request = new CommitRequest();
                var options = new ApiOptions { };

                var commits = await gitHubClient.Repository.Commit.GetAll(appGitCredential.Owner, appGitCredential.RepoName);

                var commitDetailsTasks = commits.Select(commit =>
                {
                    return System.Threading.Tasks.Task.Run(async () =>
                    {
                        var commitDetails = await gitHubClient.Repository.Commit.Get(appGitCredential.Owner, appGitCredential.RepoName, commit.Sha);
                        return new { commit, commitDetails };
                    });
                }).ToList();

                var commitResults = await System.Threading.Tasks.Task.WhenAll(commitDetailsTasks);

                foreach (var result in commitResults)
                {
                    var commit = result.commit;
                    var commitDetails = result.commitDetails;
                    var stats = commitDetails.Stats;
                    var username = commit.Author?.Login ?? commit.Committer?.Login ?? "Serhat";

                    if (!userStatsDict.TryGetValue(username, out var value))
                    {
                        value = new UserCommitStatsDto
                        {
                            UserId = username,
                            Username = username
                        };
                        userStatsDict[username] = value;
                    }

                    value.CommitCount++;
                    value.TotalAdditions += stats.Additions;
                    value.TotalDeletions += stats.Deletions;

                    foreach (var file in commitDetails.Files)
                    {
                        var estimatedRefactor = Math.Min(file.Additions, file.Deletions);
                        value.EstimatedRefactoredLines += estimatedRefactor;
                    }
                }

                return serviceResponseHelper.SetSuccess(userStatsDict.Values.ToList());
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<List<UserCommitStatsDto>>("Internal server error occurred.");
            }
        }

        public async Task<ServiceResponse<List<GitCommitDto>>> GetCommitsAsync(AppGitCredentialDto appGitCredential)
        {
            try
            {
                var gitHubClient = CreateGitHubClient(appGitCredential.PatToken);
                var commits = new List<GitCommitDto>();

                var request = new CommitRequest();
                var options = new ApiOptions { };

                var commitList = await gitHubClient.Repository.Commit.GetAll(appGitCredential.Owner, appGitCredential.RepoName);

                var commitDetailsTasks = commitList.Select(async commit =>
                {
                    var commitDetails = await gitHubClient.Repository.Commit.Get(appGitCredential.Owner, appGitCredential.RepoName, commit.Sha);

                    var filesChanged = commitDetails.Files.ToList();

                    var mappedFilesChanged = mapper.Map<List<GitCommitFileDto>>(filesChanged);

                    var gitCommit = new GitCommitDto
                    {
                        Committer = commit.Committer?.Login ?? "Serhat",
                        Additions = commitDetails.Stats?.Additions ?? 0,
                        Deletions = commitDetails.Stats?.Deletions ?? 0,
                        Message = commitDetails.Commit.Message,
                        FilesChanged = [.. mappedFilesChanged.Select(file => file.Filename)],
                        Date = commitDetails.Commit.Committer?.Date.DateTime
                    };

                    if (mappedFilesChanged.Count != 0)
                    {
                        mappedFilesChanged.ForEach(commitFile =>
                        {
                            gitCommit.AddFileChangedInfo(commitFile);
                        });
                    }

                    return gitCommit;
                }).ToList();

                var commitResults = await System.Threading.Tasks.Task.WhenAll(commitDetailsTasks);

                commits.AddRange(commitResults);

                return serviceResponseHelper.SetSuccess(commits);
            }
            catch (ApiException ex)
            {
                return serviceResponseHelper.SetError<List<GitCommitDto>>(ex?.ApiError?.Message ?? "Internal server error occurred.");
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<List<GitCommitDto>>("Internal server error occurred.");
            }
        }

        public async Task<ServiceResponse<List<UserPullRequestStatsDto>>> GetPullRequestCountsByUserAsync(AppGitCredentialDto appGitCredential)
        {
            try
            {
                var gitHubClient = CreateGitHubClient(appGitCredential.PatToken);
                var prStatsDict = new Dictionary<string, UserPullRequestStatsDto>();

                var prs = await gitHubClient.PullRequest.GetAllForRepository(appGitCredential.Owner, appGitCredential.RepoName);

                foreach (var pr in prs)
                {
                    var username = pr.User.Login;

                    if (!prStatsDict.TryGetValue(username, out var value))
                    {
                        value = new UserPullRequestStatsDto
                        {
                            UserId = username,
                            Username = username,
                            PullRequestCount = 0
                        };
                        prStatsDict[username] = value;
                    }

                    value.PullRequestCount++;
                }

                return serviceResponseHelper.SetSuccess(prStatsDict.Values.ToList());
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<List<UserPullRequestStatsDto>>("Internal server error occurred.");
            }
        }

        public async Task<ServiceResponse<List<UserPullRequestTimeStatsDto>>> GetPullRequestTimesAsync(AppGitCredentialDto appGitCredential)
        {
            try
            {
                var gitHubClient = CreateGitHubClient(appGitCredential.PatToken);
                var prTimeStatsDict = new Dictionary<string, UserPullRequestTimeStatsDto>();

                var prs = await gitHubClient.PullRequest.GetAllForRepository(appGitCredential.Owner, appGitCredential.RepoName);

                foreach (var pr in prs)
                {
                    // pull request çıkan kullanıcı adını alır
                    var username = pr.User.Login;
                    // pull request açılış ve kapanış zamanını timespan olarak hesaplar
                    var timeOpen = pr.ClosedAt - pr.CreatedAt;

                    if (timeOpen.HasValue)
                    {
                        if (!prTimeStatsDict.TryGetValue(username, out UserPullRequestTimeStatsDto? value))
                        {
                            value = new UserPullRequestTimeStatsDto
                            {
                                UserId = username,
                                Username = username,
                                PullRequestTimes = []
                            };
                            prTimeStatsDict[username] = value;
                        }

                        value.PullRequestTimes.Add(timeOpen.Value);
                    }
                }

                return serviceResponseHelper.SetSuccess(prTimeStatsDict.Values.ToList());
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<List<UserPullRequestTimeStatsDto>>("Internal server error occurred.");
            }
        }

        public async Task<ServiceResponse<List<PullRequestRecord>>> GetPullRequestsAsync(AppGitCredentialDto appGitCredential)
        {
            try
            {
                var gitHubClient = CreateGitHubClient(appGitCredential.PatToken);
                var prs = await gitHubClient.PullRequest.GetAllForRepository(appGitCredential.Owner, appGitCredential.RepoName);

                var prRecords = prs.Select(pr => new PullRequestRecord
                {
                    UserId = pr.User.Login,
                    Username = pr.User.Login,
                    Number = pr.Number,
                    Title = pr.Title,
                    Url = pr.HtmlUrl,
                    State = pr.State.StringValue,
                    Merged = pr.Merged,
                    Comments = pr.Comments,
                    CreatedAt = pr.CreatedAt.DateTime,
                    ClosedAt = pr.ClosedAt?.DateTime,
                    MergedAt = pr.MergedAt?.DateTime
                }).ToList();

                return serviceResponseHelper.SetSuccess(prRecords);
            }
            catch (ApiException ex)
            {
                return serviceResponseHelper.SetError<List<PullRequestRecord>>(ex?.ApiError?.Message ?? "Internal server error occurred.");
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<List<PullRequestRecord>>("Internal server error occurred.");
            }
        }
    }
}
