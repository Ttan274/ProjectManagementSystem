using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Controllers.Base;
using ProjectManagementSystem.ViewModel;

namespace ProjectManagementSystem.Controllers
{
    public class SprintMetricsController(ISprintService sprintService) : BaseController
    {
        public async Task<IActionResult> GetSprintSelects(SprintFilterDto sprintFilter)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(Error("Invalid request."));
            }

            if (sprintFilter.ProjectId == null)
            {
                return BadRequest(Error("Project not found."));
            }

            try
            {
                var sprintResponse = await sprintService
                    .GetListAsync(sprintFilter.ProjectId)
                    .ConfigureAwait(false);

                if (!sprintResponse.Success || sprintResponse.Data == null)
                {
                    return BadRequest(Error(sprintResponse?.ErrorMessage ?? "Sprints not found"));
                }

                var today = DateTime.Today;

                var sprintSelects = sprintResponse.Data.Select(x => new SprintSelectModel()
                {
                    Id = x.Id,
                    Name = $"{x.SprintName} ({x.StartDate:dd.MM.yyyy} - {x.FinishDate:dd.MM.yyyy})",
                    IsCurrent = (x.StartDate < today && x.FinishDate > today) ? 1 : 0
                });

                return Ok(Success(sprintSelects));
            }
            catch (Exception)
            {
                return BadRequest(Error(new List<SprintSelectModel>(), "Internal server error occured"));
            }
        }

        public async Task<IActionResult> GetOverviewMetrics(SprintFilterDto sprintFilter)
        {
            if (sprintFilter == null)
            {
                return BadRequest(Error("Invalid Request"));
            }

            sprintFilter.Validate(out string validationMessage);

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                return BadRequest(Error(validationMessage));
            }

            try
            {
                var sprintResponse = await sprintService
                    .GetSprintDetailListAsync((Guid)sprintFilter.ProjectId)
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

                var builder = new SprintOverviewMetricsBuilder(
                    sprintDetailList,
                    sprintDetailList.Find(x => x.Id == sprintFilter.Id));

                var overviewMetrics = builder
                    .WithCompletedStoryPoints()
                    .WithRemainingDays()
                    .WithOpenBugCount()
                    .WithAvgCycleTime()
                    .WithDocumentationStatus()
                    .WithCompletionRate()
                    .WithVelocity()
                    .WithOnTimeDeliveryRate()
                    .WithTotalTaskCount()
                    .WithSprintCompletionChart()
                    .Build();

                return Ok(Success(overviewMetrics));
            }
            catch (Exception)
            {
                return BadRequest(Error(new SprintOverviewMetricsDto(), "Internal server error occured"));
            }
        }
    }
}
