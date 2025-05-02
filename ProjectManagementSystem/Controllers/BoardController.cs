using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Models.Board;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Controllers
{
    public class BoardController : Controller
    {
        public IActionResult Index(int projectId)
        {
            // In a real application, you would fetch this from your database/service
            var model = new ProjectBoardViewModel(true);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateTaskStatus(string taskId, string statusId)
        {
            // In a real application:
            // 1. Validate inputs
            // 2. Update task status in database
            // 3. Return appropriate response

            return Json(new { success = true, message = "Task status updated" });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AddSubtask([FromBody] AddSubtaskRequest request)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Invalid data" });

            // In a real application:
            // 1. Validate task exists
            // 2. Create subtask in database
            // 3. Return the created subtask

            var newSubtask = new BoardSubtask
            {
                Id = Guid.NewGuid().ToString(),
                Title = request.Title,
                IsCompleted = false
            };

            return Json(new { success = true, subtask = newSubtask });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ToggleSubtask(string subtaskId, bool isCompleted)
        {
            // In a real application:
            // 1. Validate subtask exists
            // 2. Update completion status in database

            return Json(new { success = true });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteSubtask(string subtaskId)
        {
            // In a real application:
            // 1. Validate subtask exists
            // 2. Delete from database

            return Json(new { success = true });
        }

        public IActionResult GetTaskDetails(string taskId)
        {
            // In a real application, fetch from database
            var task = new BoardTask
            {
                Id = taskId,
                TaskCode = "TASK" + taskId,
                Title = "Sample Task " + taskId,
                Description = "This is a sample task description.",
                Priority = 2,
                IsUrgent = false,
                Subtasks = new List<BoardSubtask>
                {
                    new BoardSubtask("1", "First subtask", true),
                    new BoardSubtask("2", "Second subtask", false)
                },
                Assignees = new List<Assignee>
                {
                    new Assignee("1", "Alice", "/images/alice.png")
                }
            };

            return Json(task);
        }
    }

    public class AddSubtaskRequest
    {
        public string TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}