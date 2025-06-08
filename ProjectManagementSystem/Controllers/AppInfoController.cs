using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.AppInfo;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Common.ServiceResponse;
using ProjectManagementSystem.Controllers.Base;

namespace ProjectManagementSystem.Controllers
{
    public class AppInfoController(
        IProjectService projectService,
        IAppInfoService appInfoService)
        : BaseController
    {
        public async Task<IActionResult> Index(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return View(new AppProjectInfoDto());
            }

            ViewBag.ProjectId = projectId;

            try
            {
                var appProjectInfoResponse = await projectService
                    .GetAppsByProjectAsync(projectId)
                    .ConfigureAwait(false);

                if (!appProjectInfoResponse.Success)
                {
                    return View(new AppProjectInfoDto());
                }

                return View(appProjectInfoResponse.Data ?? new AppProjectInfoDto());
            }
            catch (Exception)
            {
                return View(new AppProjectInfoDto());
            }
        }

        public async Task<IActionResult> Action(AppInfoDto appInfoDto)
        {
            if (!ModelState.IsValid || appInfoDto == null || appInfoDto.ProjectId == null || appInfoDto.ProjectId == Guid.Empty)
            {
                return BadRequest(Error("Invalid request"));
            }

            if (appInfoDto.Id == Guid.Empty)
            {
                return PartialView("_Action", new CreateAppInfoDto() { ProjectId = appInfoDto.ProjectId });
            }

            try
            {
                var appInfoResponse = await appInfoService
                    .GetByIdAsync(appInfoDto.Id)
                    .ConfigureAwait(false);

                if (appInfoResponse == null || !appInfoResponse.Success || appInfoResponse.Data == null)
                {
                    return BadRequest(Error(appInfoResponse?.ErrorMessage ?? "App info not found."));
                }

                return PartialView("_Action", appInfoResponse.Data);
            }
            catch (Exception)
            {
                return PartialView(new CreateAppInfoDto());
            }
        }

        [HttpPost]
        public async Task<IActionResult> Action([FromBody] CreateAppInfoDto createDto)
        {
            if (!ModelState.IsValid || createDto == null)
            {
                return BadRequest(Error("Invalid request"));
            }

            ServiceResponse<AppInfoDto> appInfoResponse;

            try
            {
                if (createDto.Id != Guid.Empty)
                {
                    appInfoResponse = await appInfoService
                        .UpdateAsync((UpdateAppInfoDto)createDto)
                        .ConfigureAwait(false);
                }
                else
                {
                    appInfoResponse = await appInfoService
                        .CreateAsync(createDto)
                        .ConfigureAwait(false);
                }

                if (!appInfoResponse.Success || appInfoResponse.Data == null)
                {
                    return BadRequest(Error(appInfoResponse?.ErrorMessage ?? "App info not found."));
                }

                return Ok(Success(appInfoResponse.Data));
            }
            catch (Exception)
            {
                return PartialView(new CreateAppInfoDto());
            }
        }
    }
}
