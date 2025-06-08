using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Application.Abstractions.User.Dtos;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Models;
using ProjectManagementSystem.Services.Mail;
using ProjectManagementSystem.ViewModel;

namespace ProjectManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILoginService _loginService;
        private readonly ITeamService _teamService;
        private readonly IEmailSender _emailSender;

        public UserController(IUserService userService,
            UserManager<AppUser> userManager,
            ILoginService loginService,
            ITeamService teamService,
            IEmailSender emailSender)
        {
            _userService = userService;
            _userManager = userManager;
            _loginService = loginService;
            _teamService = teamService;
            _emailSender = emailSender;
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
            if (!ModelState.IsValid) return View(userDto);

            var user = await _userManager.FindByEmailAsync(userDto.Email);

            if (user is not null && user.MustChangePassword)
            {
                //buraya toast eklenecek
                return RedirectToAction("ChangePassword", new { userId = user.Id });
            }

            var result = await _loginService.Login(userDto);
            var isAdmin = await _loginService.CheckRole(userDto.Email, "Admin");

            if (result)
            {
                if (isAdmin)
                    return RedirectToAction("AdminPage", "User");
                else
                    return RedirectToAction("DeptMain", "Dept");
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(string? userId = null)
        {
            var model = new ChangePasswordViewModel();

            if (string.IsNullOrEmpty(userId))
                return View(model);

            var user = await _userService.FindById(userId);

            model.Email = user.Email!;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (model.Email is null || model.OldPassword is null || model.Password is null)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction("Error");

            var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.Password);
            if (result.Succeeded)
            {
                user.MustChangePassword = false;

                await _userManager.UpdateAsync(user);

                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public IActionResult ResetPasswordRequest()
        {
            var model = new ResetPasswordRequestModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ResetPasswordRequest(ResetPasswordRequestModel model)
        {
            if (model is null)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user is null) //toast mesaj 
                return View(model);

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink = Url.Action("ResetPassword", "User", new
            {
                userId = user.Id.ToString(),
                token
            }, protocol: Request.Scheme);

            await _emailSender.SendEmailAsync(user.Email!, "Reset Password",
                "Hello,<br/><br/>" +
                $"To access the system with {user.Email}, you can use the connection to reset your password<br/><br/>" +
                $"<a href='{resetLink}'>Reset Password</a><br/><br/>" +
                "If you did not request this, please ignore this email."
            );

            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(string userId, string token)
        {
            var model = new ResetPasswordModel();

            if (string.IsNullOrEmpty(userId))
                return View(model);

            var user = await _userService.FindById(userId);

            model.Email = user.Email!;
            model.Token = token;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ResetPasswordModel model)
        {
            if (model.Email is null || model.Token is null)
                return View(model);

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null) return RedirectToAction("Error");

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);
            if (result.Succeeded)
            {
                user.MustChangePassword = false;

                await _userManager.UpdateAsync(user);

                return RedirectToAction("Login");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
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
            if (ModelState.IsValid)
            {
                if (viewModel.UserToCreate is null)
                    return View();

                var randomPassword = GenerateRandomPassword();
                viewModel.UserToCreate.Password = randomPassword;

                var user = await _userService.AddUser(viewModel.UserToCreate);

                if (user is null)
                    return View();

                var resetLink = Url.Action("ChangePassword", "User", new
                {
                    userId = user.Id.ToString(),
                }, protocol: Request.Scheme);

                await _emailSender.SendEmailAsync(user.Email!, "Set Your Password",
                    "Hello,<br/><br/>" +
                    $"To access the system with {user.Email}, you need to change your password first. Please click the link below to set your password:<br/><br/>" +
                    $"Your temporary password: {randomPassword}<br/><br/>" +
                    $"<a href='{resetLink}'>Set Password</a><br/><br/>" +
                    "If you did not request this, please ignore this email."
                );
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
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteDept(Guid id)
        {
            await _teamService.DeleteTeam(id);
            return RedirectToAction("AdminPage", "User");
        }
        #endregion
        public static string GenerateRandomPassword(int length = 12)
        {
            const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*?_-";
            var random = new Random();
            return new string(Enumerable.Repeat(validChars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
