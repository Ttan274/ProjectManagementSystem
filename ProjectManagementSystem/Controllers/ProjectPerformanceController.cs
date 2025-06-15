using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig;
using ProjectManagementSystem.Controllers.Base;

namespace ProjectManagementSystem.Controllers
{
    public class ProjectPerformanceController(IProjectTeamConfigService projectTeamConfigService) : BaseController
    {
        public async Task<IActionResult> Index(Guid projectId)
        {
            ViewBag.ProjectId = projectId;

            var projectConfigResponse = await projectTeamConfigService
                .GetByProjectIdAsync(projectId)
                .ConfigureAwait(false);

            ViewBag.HasProjectConfigured = projectConfigResponse.Success;

            return View();
        }
    }
}
