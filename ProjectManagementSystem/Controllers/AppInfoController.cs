using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.AppInfo;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.Project;
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
    }
}
