using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using ProjectManagementSystem.Application.Abstractions.AppInfo;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto;
using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.Team.Dto;
using ProjectManagementSystem.Controllers.Base;

namespace ProjectManagementSystem.Controllers
{
    public class TeamController(
        ITeamService teamService,
        ISprintService sprintService,
        IAppInfoService appInfoService,
        IGitHubRepoAnalyticsService repoAnalyticsService,
        IProjectTeamConfigService projectTeamConfigService,
        IMemoryCache memoryCache,
        IMapper mapper) : BaseController
    {
        private const string CommitStatsCacheKey = "CommitStats_{0}";
        public async Task<IActionResult> GetTeamMemberPerformances(FilterTeamMemberPerformanceDto filterRequest)
        {
            if (filterRequest == null)
            {
                return BadRequest(Error("Invalid Request"));
            }

            try
            {
                var response = await teamService
                    .GetTeamMemberPerformancesAsync(filterRequest)
                    .ConfigureAwait(false);

                if (!response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception)
            {
                return BadRequest(Error("Internal server error occured"));
            }
        }

        public async Task<IActionResult> GetTeamPerformace(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return BadRequest(Error("Invalid request"));
            }

            try
            {
                if (memoryCache.TryGetValue(string.Format(CommitStatsCacheKey, projectId), out List<GitCommitDto> cachedGitCommits))
                {
                    return await BuildTeamPerformance(cachedGitCommits, projectId);
                }

                var sprintResponse = await sprintService
                    .GetSprintDetailListAsync(projectId)
                    .ConfigureAwait(false);

                if (!sprintResponse.Success)
                {
                    return BadRequest(BadRequest());
                }

                var sprintDetailList = sprintResponse.Data;

                if (sprintDetailList == null)
                {
                    return BadRequest(Error(sprintDetailList, "Sprint details not found."));
                }

                var applicationResponse = await appInfoService.GetListAsync();

                if (!applicationResponse.Success)
                {
                    return BadRequest(Error(applicationResponse?.ErrorMessage ?? "Internal server error occurred."));
                }

                var applications = applicationResponse.Data;

                var gitCredentials = mapper.Map<List<AppGitCredentialDto>>(applications.Where(x => x.ProjectId == projectId));

                List<GitCommitDto> gitCommits = new();

                var tasks = gitCredentials.Select(async gitCredential =>
                {
                    var repoAnalyticsResponse = await repoAnalyticsService
                        .GetCommitsAsync(gitCredential)
                        .ConfigureAwait(false);

                    return repoAnalyticsResponse.Data ?? new List<GitCommitDto>();
                }).ToList();

                var result = await Task.WhenAll(tasks).ConfigureAwait(false);

                foreach (var commits in result)
                {
                    gitCommits.AddRange(commits);
                }

                memoryCache.Set(string.Format(CommitStatsCacheKey, projectId), gitCommits, TimeSpan.FromMinutes(10));

                var projectTeamConfigResponse = await projectTeamConfigService
                    .GetByProjectIdAsync(projectId)
                    .ConfigureAwait(false);

                var analysisResult = new CommitAnalysisBuilder(gitCommits, projectTeamConfigResponse.Data)
                    .AddCommitStatAnalysis()
                    .Build();

                var commitStats = analysisResult.CommitStatAnalysis?.CommitStats;

                var builder = new TeamPerformanceBuilder(sprintDetailList, commitStats, projectTeamConfigResponse.Data);

                var overviewMetrics = builder.Build();

                return Ok(Success(overviewMetrics));
            }
            catch (Exception)
            {
                return BadRequest(Error(new SprintOverviewMetricsDto(), "Internal server error occurred"));
            }
        }

        private async Task<IActionResult> BuildTeamPerformance(List<GitCommitDto> gitCommits, Guid projectId)
        {
            var sprintResponse = await sprintService
                .GetSprintDetailListAsync(projectId)
                .ConfigureAwait(false);

            var sprintDetailList = sprintResponse.Data;

            var applicationResponse = await appInfoService.GetListAsync();

            var applications = applicationResponse.Data;

            var projectTeamConfigResponse = await projectTeamConfigService
                .GetByProjectIdAsync(projectId)
                .ConfigureAwait(false);

            var analysisResult = new CommitAnalysisBuilder(gitCommits, projectTeamConfigResponse.Data)
                .AddCommitStatAnalysis()
                .Build();

            var commitStats = analysisResult.CommitStatAnalysis?.CommitStats;

            var builder = new TeamPerformanceBuilder(sprintDetailList, commitStats, projectTeamConfigResponse.Data);

            var overviewMetrics = builder.Build();

            return Ok(Success(overviewMetrics));
        }
    }
}
