using Octokit;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto;
using ProjectManagementSystem.Common.ServiceResponse;
using ProductHeaderValue = Octokit.ProductHeaderValue;

namespace ProjectManagementSystem.Application.GitHubRepoAnalytics
{
    public class GitHubRepoAnalyticsService(IServiceResponseHelper serviceResponseHelper) : IGitHubRepoAnalyticsService
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
                var options = new ApiOptions { PageSize = 100 };

                var commits = await gitHubClient.Repository.Commit.GetAll(appGitCredential.Owner, appGitCredential.RepoName, request, options);

                foreach (var commit in commits)
                {
                    var username = commit.Author?.Login ?? commit.Committer?.Login ?? "Serhat";

                    var commitDetails = await gitHubClient.Repository.Commit.Get(appGitCredential.Owner, appGitCredential.RepoName, commit.Sha);
                    var stats = commitDetails.Stats;

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
                }

                return serviceResponseHelper.SetSuccess(userStatsDict.Values.ToList());
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<List<UserCommitStatsDto>>("Internal server error occurred.");
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
    }
}
