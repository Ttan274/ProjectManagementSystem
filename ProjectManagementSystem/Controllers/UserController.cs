using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Application.Abstractions.User.Dtos;

namespace ProjectManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILoginService _loginService;

        public UserController(IUserService userService, ILoginService loginService)
        {
            _userService = userService;
            _loginService = loginService;
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
                    return RedirectToAction("Index", "Home");
            }

            return View();
        }
        #endregion

        #region AdminRegion
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AdminPage()
        {
            var admin = await _userService.GetAdmin(User);
            return View(admin);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ListUsers()
        {
            var users = await _userService.GetAllUsers();
            users.Sort((x, y) => DateTime.Compare(y.CreatedDate, x.CreatedDate));
            return View(users);
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateUser(UserDto userDto)
        {
            if(ModelState.IsValid)
            {
                var response = await _userService.AddUser(userDto);

                if (response)
                    return RedirectToAction("ListUsers", "User");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            await _userService.DeleteUser(id);
            return RedirectToAction("ListUsers", "User");
        }
        #endregion
    }
}
