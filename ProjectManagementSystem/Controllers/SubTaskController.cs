using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.SubTaskProducer;
using ProjectManagementSystem.Models.SubTask;

namespace ProjectManagementSystem.Controllers
{
    public class SubTaskController : Controller
    {
        private readonly ISubTaskProducerService _subTaskProducerService;
        public SubTaskController(ISubTaskProducerService _subTaskProducerService)
        {
            this._subTaskProducerService = _subTaskProducerService;
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
    }
}
