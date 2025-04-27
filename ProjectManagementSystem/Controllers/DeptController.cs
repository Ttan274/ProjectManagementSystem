using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.User;

namespace ProjectManagementSystem.Controllers
{
    public class DeptController : Controller
    {
        //private readonly ProjectService _projectService;
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;

        public DeptController(ITeamService teamService, IUserService userService)
        {
           // _projectService = projectService;
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
    }
}
