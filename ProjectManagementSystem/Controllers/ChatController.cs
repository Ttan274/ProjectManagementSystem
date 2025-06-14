using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Chat;
using ProjectManagementSystem.Application.Abstractions.User;

namespace ProjectManagementSystem.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IUserService _userService;

        public ChatController(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<IActionResult> Chat()
        {
            var userId = await _userService.GetCurrentUserId(User);
            var teamId = await _userService.GetTeamId(User);
            var users = await _userService.GetAllUsersExceptCurrent(User, teamId);

            ViewBag.userId = userId;
            return View(users);
        }
    }
}
