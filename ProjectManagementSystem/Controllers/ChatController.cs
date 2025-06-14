using Microsoft.AspNetCore.Mvc;

namespace ProjectManagementSystem.Controllers
{
    public class ChatController : Controller
    {
        public IActionResult Chat()
        {
            return View();
        }
    }
}
