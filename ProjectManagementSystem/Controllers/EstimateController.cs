using System.Globalization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementSystem.Application.Abstractions.Estimate;
using ProjectManagementSystem.Application.Abstractions.Estimate.Dto;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Task;
using ProjectManagementSystem.Common.Enums;

namespace ProjectManagementSystem.Controllers
{
    [Authorize]
    public class EstimateController(
        IEstimateService estimateService,
        ISprintService sprintService,
        ITaskService taskService
        ) : Controller
    {
        public async Task<IActionResult> Index(Guid projectId) 
        {
            var result = await estimateService.GetAllByProjectIdAsync(projectId);
            
            if (!result.Success)
                return RedirectToAction("Index", "Board", new { projectId = projectId });

            return View(result.Data);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid projectId)
        {
            var sprints = await sprintService.GetAllSprintsByProjectId(projectId);

            var model = new CreateEstimateDto()
            {
                ProjectId = projectId,
                SprintId = sprints.FirstOrDefault()?.SprintName ?? string.Empty,
                SprintList = sprints.Select(x => new SelectListItem { Text = x.SprintName, Value = x.Id.ToString() }).ToList()
            };

            return PartialView("_Create", model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateEstimateDto model)
        {
            var result = await estimateService.CreateAsync(model);

            if (!result.Success)
                return View(model);

            return RedirectToAction("Index", new { projectId = model.ProjectId });
        }

        public async Task<IActionResult> TaskEstimate(Guid estimateId)
        {
            var result = await estimateService.GetEstimateTasksInfoAsync(estimateId);

            return View(result.Data);
        }

        public async Task<IActionResult> TaskDetail(Guid taskId, int type)
        {
            var result = await taskService.GetEstimateTask(taskId);

            if (result.Data is not null)
                result.Data.EstimatePoints = GetEstimatePoint(type);

            return PartialView("_TaskDetail", result.Data);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEffort(Guid taskId, string effort)
        {
            double parsed = double.Parse(effort, CultureInfo.InvariantCulture);

            var result = await taskService.UpdateTaskEffort(taskId, Convert.ToInt32(parsed));

            return Json(new { status = result.Data });
        }


        private static int[] GetEstimatePoint(int type)
            => type switch
            {
                (int)EstimateType.ZeroToFive => [1, 2, 3, 4, 5],
                (int)EstimateType.ZeroToTen => [1, 2, 3, 4, 5, 6, 7, 8, 9, 10],
                (int)EstimateType.Doubling => [1, 2, 4, 8, 16, 32, 64, 128],
                (int)EstimateType.Fibonacci => [1, 2, 3, 5, 8, 13, 21, 34, 55, 89],
                _ => [1, 2, 3, 4, 5, 6, 7, 8, 9, 10]
            };
    }
}
