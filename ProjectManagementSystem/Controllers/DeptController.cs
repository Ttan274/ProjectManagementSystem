using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.Team.Dto;
using ProjectManagementSystem.Application.Abstractions.User;

namespace ProjectManagementSystem.Controllers
{
    public class DeptController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;

        public DeptController(IProjectService projectService, ITeamService teamService, IUserService userService)
        {
            _projectService = projectService;
            _teamService = teamService;
            _userService = userService;
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeptMain()
        {
            var teamId = await _userService.GetTeamId(User);
            var team = await _teamService.GetTeamById(Guid.Parse(teamId));

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
