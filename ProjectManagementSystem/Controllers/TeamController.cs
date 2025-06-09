using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.AppInfo;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto;
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
        IMapper mapper) : BaseController
    {
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
                var sprintResponse = await sprintService
                    .GetSprintDetailListAsync(projectId)
                    .ConfigureAwait(false);

                if (!sprintResponse.Success)
                {
                    return BadRequest(BadRequest());
                }

                var sprintDetailList = sprintResponse.Data ?? [];

                if (sprintDetailList == null)
                {
                    return BadRequest(Error(sprintDetailList, "Sprint details not found."));
                }

                var applicationResponse = await appInfoService.GetListAsync();

                if (!applicationResponse.Success)
                {
                    return BadRequest(Error(applicationResponse?.ErrorMessage ?? "Internal server error occured."));
                }

                var applications = applicationResponse.Data ?? [];

                var gitCredentials = mapper.Map<List<AppGitCredentialDto>>(applications.Where(x => x.ProjectId == projectId));

                List<GitCommitDto> gitCommits = [];

                foreach (var gitCredential in gitCredentials)
                {
                    var repoAnalyticsResponse = await repoAnalyticsService
                        .GetCommitsAsync(gitCredential)
                        .ConfigureAwait(false);

                    gitCommits.AddRange(repoAnalyticsResponse.Data ?? []);
                }

                var analysisResult = new CommitAnalysisBuilder(gitCommits).AddCommitStatAnalysis().Build();

                var commitStats = analysisResult.CommitStatAnalysis?.CommitStats ?? [];

                var builder = new TeamPerformanceBuilder(sprintDetailList, commitStats);

                var overviewMetrics = builder.Build();

                return Ok(Success(overviewMetrics));
            }
            catch (Exception)
            {
                return BadRequest(Error(new SprintOverviewMetricsDto(), "Internal server error occured"));
            }
        }
    }
}
