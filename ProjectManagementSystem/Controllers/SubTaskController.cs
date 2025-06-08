using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Dto;
using ProjectManagementSystem.Application.Abstractions.SubTaskProducer;
using ProjectManagementSystem.Application.Abstractions.Task;
using ProjectManagementSystem.Models.SubTask;

namespace ProjectManagementSystem.Controllers
{
    public class SubTaskController : Controller
    {
        private readonly ISubTaskProducerService _subTaskProducerService;
        private readonly ITaskService _taskService;
        public SubTaskController(ISubTaskProducerService _subTaskProducerService,
            ITaskService _taskService)
        {
            this._subTaskProducerService = _subTaskProducerService;
            this._taskService = _taskService;
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateByOllama([FromBody] CreateSubTaskRequestModel createSubTaskModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Geçersiz istek");
            }

            var subTaskResponse = await _subTaskProducerService
                .GenerateSubTasksAsync(createSubTaskModel.Description)
                .ConfigureAwait(false);

            if (subTaskResponse == null) {
                return BadRequest("İstek başarısız oldu");
            }

            return Ok(subTaskResponse);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] CreateSubTaskDto model)
        {
            if (model is null)
                return BadRequest("Error");

            var id = await _taskService.CreateSubtask(model);

            return Json(new { depId = id });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Delete([FromBody] DeleteSubTaskDto model)
        {
            if (model is null)
                return BadRequest("Error");

            await _taskService.DeleteSubtask(model);

            return Json(new { status = "ok" });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Update([FromBody] UpdateSubTaskDto model)
        {
            if (model is null)
                return BadRequest("Error");

            await _taskService.UpdateSubtask(model);

            return Json(new { status = "ok" });
        }
        
    }
}
