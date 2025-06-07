using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Controllers.Base;

namespace ProjectManagementSystem.Controllers
{
    public class SprintMetricsController(ISprintService sprintService) : BaseController
    {
        public async Task<IActionResult> GetOverviewMetrics(Guid sprintId)
        {
            if (sprintId == Guid.Empty)
            {
                return BadRequest(Error("Invalid Request"));
            }

            try
            {
                var sprintResponse = await sprintService
                    .GetSprintDetailsAsync(sprintId)
                    .ConfigureAwait(false);

                if (!sprintResponse.Success)
                {
                    return BadRequest(BadRequest());
                }

                var sprintDetails = sprintResponse.Data;

                if (sprintDetails == null)
                {
                    return BadRequest(Error(sprintDetails, "Sprint details not found."));
                }

                var builder = new SprintOverviewMetricsBuilder(sprintDetails);

                var overviewMetrics = builder
                    .WithCompletedStoryPoints()
                    .WithRemainingDays()
                    .WithOpenBugCount()
                    .WithAvgCycleTime()
                    .WithDocumentationStatus()
                    .Build();

                return Ok(overviewMetrics);
            }
            catch (Exception)
            {
                return BadRequest(Error(new SprintOverviewMetricsDto(), "Internal server error occured"));
            }
        }

        public async Task<IActionResult> GetSprintProgressChart(Guid sprintId)
        {
            if (sprintId == Guid.Empty)
            {
                return BadRequest(Error("Invalid Request"));
            }

            try
            {
                var sprintResponse = await sprintService
                    .GetSprintDetailsAsync(sprintId)
                    .ConfigureAwait(false);

                if (!sprintResponse.Success)
                {
                    return BadRequest(BadRequest());
                }

                var sprintDetails = sprintResponse.Data;

                if (sprintDetails == null)
                {
                    return BadRequest(Error(sprintDetails, "Sprint details not found."));
                }

                var builder = new SprintProgressChartDto.Builder(sprintDetails);

                var chart = builder.Build();

                return Ok(sprintResponse);
            }
            catch (Exception)
            {
                return BadRequest(Error(new SprintOverviewMetricsDto(), "Internal server error occured"));
            }
        }
    }
}
