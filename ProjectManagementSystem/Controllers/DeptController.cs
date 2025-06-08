using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.Team.Dto;

namespace ProjectManagementSystem.Controllers
{
    public class DeptController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ITeamService _teamService;

        public DeptController(IProjectService projectService, ITeamService teamService)
        {
            _projectService = projectService;
            _teamService = teamService;
        }

        [Authorize(Roles = "Employee")]
        public IActionResult Index()
        {
            return RedirectToAction("DeptMain");
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeptMain()
        {
            var team = await _teamService.GetTeamByUser(User);
            return View(team);
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeptCrew()
        {
            var team = await _teamService.GetTeamByUser(User);
            return View(team);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateProject(TeamDto teamDto)
        {
            if(ModelState.IsValid)
            {
                var response = await _projectService.CreateProject(teamDto.ProjectDto);

                if (response)
                    return RedirectToAction("DeptMain", "Dept");
            }

            return View();
        }
    }
}
