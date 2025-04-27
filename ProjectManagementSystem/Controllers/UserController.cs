using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Application.Abstractions.User.Dtos;
using ProjectManagementSystem.ViewModel;

namespace ProjectManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;
        private readonly ITeamService _teamService;

        public UserController(IUserService userService, ILoginService loginService, ITeamService teamService)
        {
            _userService = userService;
            _loginService = loginService;
            _teamService = teamService;
        }

        #region AnonymousUser
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(UserDto userDto)
        {
            if(!ModelState.IsValid) return View(userDto);

            var result = await _loginService.Login(userDto);
            var isAdmin = await _loginService.CheckRole(userDto.Email, "Admin");

            if(result)
            {
                if (isAdmin)
                    return RedirectToAction("AdminPage", "User");
                else
                    return RedirectToAction("DeptMain", "Dept");
            }

            return View();
        }
        #endregion

        #region AdminRegion
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPage()
        {
            var users = await _userService.GetAllUsers();
            users.Sort((x, y) => DateTime.Compare(y.CreatedDate, x.CreatedDate));

            var teams = await _teamService.GetAllTeams();
            teams.Sort((x, y) => DateTime.Compare(y.CreatedDate, x.CreatedDate));

            var depts = teams.Select(x => new SelectListItem() { Text = x.TeamName, Value = x.Id.ToString() });

            AdminViewModel adminViewModel = new AdminViewModel();
            adminViewModel.Users = users;
            adminViewModel.Teams = teams;
            adminViewModel.UserToCreate = new()
            {
                TeamList = depts.ToList(),
            };
            

            return View(adminViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(AdminViewModel viewModel)
        {
            if(ModelState.IsValid)
            {
                var response = await _userService.AddUser(viewModel.UserToCreate);

                if (response)
                    return RedirectToAction("AdminPage", "User");
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUser(id);
            return RedirectToAction("AdminPage", "User");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateDept(AdminViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var response = await _teamService.CreateTeam(viewModel.TeamToCreate);

                if (response)
                    return RedirectToAction("AdminPage", "User");
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDept(Guid id)
        {
            await _teamService.DeleteTeam(id);
            return RedirectToAction("AdminPage", "User");
        }
        #endregion
    }
}
