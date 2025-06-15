using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig;
using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig.Dto;
using ProjectManagementSystem.Controllers.Base;

namespace ProjectManagementSystem.Controllers
{
    public class ProjectTeamConfigController(
        IProjectTeamConfigService projectTeamConfigService)
        : BaseController
    {
        [HttpPost()]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] ProjectTeamConfigDto projectTeamConfig)
        {
            if (!ModelState.IsValid || projectTeamConfig == null)
            {
                return BadRequest(Error("Invalid request."));
            }

            projectTeamConfig.Validate(out string validationMessage);

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                return BadRequest(Error(validationMessage));
            }

            try
            {
                var createResponse = await projectTeamConfigService
                    .CreateAsync(projectTeamConfig)
                    .ConfigureAwait(false);

                if (!createResponse.Success)
                {
                    return BadRequest(createResponse);
                }

                return Ok(createResponse);
            }
            catch (Exception)
            {
                return BadRequest(Error("Internal server error occured."));
            }
        }

        [HttpPost()]
        [ValidateAntiForgeryToken]
        public IActionResult Update([FromBody] ProjectTeamConfigDto projectTeamConfig)
        {
            if (!ModelState.IsValid || projectTeamConfig == null)
            {
                return BadRequest(Error("Invalid request."));
            }

            projectTeamConfig.Validate(out string validationMessage);

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                return BadRequest(Error(validationMessage));
            }

            try
            {
                var updateResponse = projectTeamConfigService
                    .Update(projectTeamConfig);

                if (!updateResponse.Success)
                {
                    return BadRequest(updateResponse);
                }

                return Ok(updateResponse);
            }
            catch (Exception)
            {
                return BadRequest(Error("Internal server error occured."));
            }
        }

        [HttpGet()]
        public async Task<IActionResult> GetById(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(Error("Invalid request."));
            }

            try
            {
                var configResponse = await projectTeamConfigService
                    .GetByIdAsync(id)
                    .ConfigureAwait(false);

                if (!configResponse.Success)
                {
                    return BadRequest(Error(configResponse.ErrorMessage ?? "Error occured"));
                }

                return Ok(configResponse);
            }
            catch (Exception)
            {
                return BadRequest(Error("Internal server error occured."));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Action(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return BadRequest(Error("Invalid request."));
            }

            try
            {
                var configResponse = await projectTeamConfigService
                    .GetByProjectIdAsync(projectId)
                    .ConfigureAwait(false);

                var projectConfig = configResponse.Data ?? new ProjectTeamConfigDto();

                projectConfig.ProjectId = projectId;

                return PartialView("_Action", projectConfig);
            }
            catch (Exception)
            {
                return BadRequest(Error("Interval server error occured."));
            }
        }
    }
}
