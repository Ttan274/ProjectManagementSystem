using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.AppInfo;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto;
using ProjectManagementSystem.Controllers.Base;

namespace ProjectManagementSystem.Controllers
{
    public class GithubAnalyticsController(
        IMapper mapper,
        IGitHubRepoAnalyticsService gitHubRepoAnalyticsService,
        IAppInfoService appInfoService)
        : BaseController
    {
        public async Task<IActionResult> GetCommitStats(Guid appId)
        {
            if (appId == Guid.Empty)
            {
                return BadRequest(Error("Invalid request."));
            }

            try
            {
                var appInfoResponse = await appInfoService
                    .GetByIdAsync(appId)
                    .ConfigureAwait(false);

                if (!appInfoResponse.Success)
                {
                    return BadRequest(appInfoResponse);
                }

                var appInfo = appInfoResponse.Data;

                var gitCredentials = mapper.Map<AppGitCredentialDto>(appInfo);

                var commitStatsResponse = await gitHubRepoAnalyticsService
                    .GetCommitsAsync(gitCredentials)
                    .ConfigureAwait(false);

                if (!commitStatsResponse.Success)
                {
                    return BadRequest(commitStatsResponse);
                }

                if (commitStatsResponse.Data == null)
                {
                    return BadRequest(Error("No commit found."));
                }

                var analysisResult = new CommitAnalysisBuilder(commitStatsResponse.Data)
                .AddAllAnalyses()
                .Build();

                return Ok(analysisResult);
            }
            catch (Exception)
            {
                return BadRequest(Error("Internal server error occured."));
            }
        }

        public async Task<IActionResult> GetPullRequestStats(Guid appId)
        {
            if (appId == Guid.Empty)
            {
                return BadRequest(Error("Invalid request."));
            }

            try
            {
                var appInfoResponse = await appInfoService
                    .GetByIdAsync(appId)
                    .ConfigureAwait(false);

                if (!appInfoResponse.Success)
                {
                    return BadRequest(appInfoResponse);
                }

                var appInfo = appInfoResponse.Data;

                var gitCredentials = mapper.Map<AppGitCredentialDto>(appInfo);

                var pullRequestsResponse = await gitHubRepoAnalyticsService
                    .GetPullRequestsAsync(gitCredentials)
                    .ConfigureAwait(false);

                if (!pullRequestsResponse.Success)
                {
                    return BadRequest(Error(pullRequestsResponse?.ErrorMessage ?? "Internal server error occured."));
                }

                var pullRequests = pullRequestsResponse.Data ?? [];

                if (pullRequests.Count == 0)
                {
                    // kayıt yoksa boş döner
                    return Ok(Success(new PullRequestAnalysisResult()));
                }

                var prAnalysis = new PullRequestAnalysisBuilder(pullRequests)
                     .WithFastestClosedPullRequests(TimeSpan.FromHours(1))
                     .WithSlowestClosedPullRequests(TimeSpan.FromDays(7))
                     .WithStaleOpenPullRequests(TimeSpan.FromDays(30))
                     .WithMostCommentedPullRequests(5)
                     .WithAverageCommentCount()
                     .WithPullRequestsWithoutComments()
                     .WithTotalOpenCount()
                     .WithTotalClosedCount()
                     .WithTotalMergedCount()
                     .WithOpenPullRequests()
                     .WithMergedPullRequests()
                     .WithPullRequestsPerWeek()
                     .WithMostActiveWeeks()
                     .WithLeastActiveWeeks()
                     .Build();

                return Ok(Success(prAnalysis));
            }
            catch (Exception)
            {
                return BadRequest(Error("Internal server error occured."));
            }
        }
    }
}
