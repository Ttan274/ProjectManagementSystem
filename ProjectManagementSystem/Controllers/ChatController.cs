using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Chat;
using ProjectManagementSystem.Application.Abstractions.User;

namespace ProjectManagementSystem.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IUserService _userService;

        public ChatController(IChatService chatService, IUserService userService)
        {
            _chatService = chatService;
            _userService = userService;
        }

        public async Task<IActionResult> Chat()
        {
            var currentId = User.FindFirst("Id").Value;

            var teamId = await _userService.GetTeamId(User);
            var users = await _userService.GetAllUsersByTeamId(Guid.Parse(teamId));


            return View(users);
        }
    }
}
