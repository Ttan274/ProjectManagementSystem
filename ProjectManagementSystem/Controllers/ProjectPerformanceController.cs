using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Controllers.Base;

namespace ProjectManagementSystem.Controllers
{
    public class ProjectPerformanceController : BaseController
    {
        public IActionResult Index(Guid projectId)
        {
            ViewBag.ProjectId = projectId;

            return View();
        }
    }
}
