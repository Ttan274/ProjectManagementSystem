using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Task;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Common.Enums;
using ProjectManagementSystem.Models.Board;

namespace ProjectManagementSystem.Controllers
{
    public class BoardController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly ISprintService _sprintService;
        private readonly IUserService _userService;

        public BoardController(ITaskService taskService,
            IProjectService projectService,
            ISprintService sprintService,
            IUserService userService)
        {
            _taskService = taskService;
            _projectService = projectService;
            _sprintService = sprintService;
            _userService = userService;
        }
        public async Task<IActionResult> Index(Guid projectId)
        {
            var sprints = await _sprintService.GetAllSprintsByProjectId(projectId);
            var lastSprint = sprints.FirstOrDefault();

            var model = await PrepareBoardModelAsync(projectId, sprints, lastSprint?.Id);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BoardRefresh(Guid projectId, Guid sprintId)
        {
            var sprints = await _sprintService.GetAllSprintsByProjectId(projectId);
            var model = await PrepareBoardModelAsync(projectId, sprints, sprintId);

            return PartialView("Board", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateTaskStatus(string taskId, string statusId)
        {
            await _taskService.UpdateTaskStatus(taskId, statusId);

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
                Assigned = new Assignee("1", "Alice", "/images/alice.png")
            };

            return Json(task);
        }


        [HttpPost]
        public async Task<IActionResult> AssignUser([FromBody]AssignUserModel request)
        {
            var task = await _taskService.UpdateTaskUser(request.TaskId, request.UserId);

            return Json(new { success = true, message = "Task user updated" });
        }

        private async Task<ProjectBoardViewModel> PrepareBoardModelAsync(Guid projectId, IEnumerable<SprintDto> sprints, Guid? selectedSprintId)
        {
            var model = new ProjectBoardViewModel
            {
                ProjectId = projectId
            };

            var project = await _projectService.GetProjectById(projectId);
            model.ProjectName = project.ProjectName!;
            model.SprintList = CreateSprintSelectList(sprints);

            if (selectedSprintId.HasValue)
            {
                model.SprintId = selectedSprintId.Value.ToString();

                var tasks = await _taskService.GetAllTasksBySprintId(selectedSprintId.Value);
                model.Columns = CreateBoardColumns(tasks);

                var currentSprint = sprints.FirstOrDefault(x => x.Id == selectedSprintId.Value);
                model.SprintInterval = $"{currentSprint?.StartDate} - {currentSprint?.FinishDate}";

                var users = await _userService.GetAllUsersByTeamId(project.TeamId);
                model.UserList.Add(new SelectListItem { Text = "Lütfen Seçiniz", Value = string.Empty });
                model.UserList.AddRange(users.Select(x => new SelectListItem
                {
                    Text = $"{x.Name}{x.Surname} ({x.UserName})",
                    Value = x.Id.ToString()
                }));

                model.TeamId = project.TeamId;
            }
            return model;
        }

        private List<BoardColumn> CreateBoardColumns(IEnumerable<TaskDto> tasks)
        {
            return Enum.GetValues(typeof(ProjectStatus))
                .Cast<ProjectStatus>()
                .Select(status => new BoardColumn(
                    ((int)status).ToString(),
                    status.ToString().Replace("InProgress", "In Progress"),
                    tasks.Where(t => t.State == status && (t.TaskId == null || t.TaskId == Guid.Empty))
                         .Select(t => new BoardTask
                         {
                             Id = t.Id.ToString(),
                             TaskCode = "TSK",
                             Title = t.TaskName ?? string.Empty,
                             Description = t.TaskName ?? string.Empty,
                             Priority = t.Priority != null ? (int)t.Priority : 0,
                             IsUrgent = false,
                             Subtasks = new List<BoardSubtask>
                             {
                                new BoardSubtask("1", "Create wireframes", false),
                                new BoardSubtask("2", "Design color scheme", true)
                             },
                             UserId = t.AppUser != null ? t.AppUser.Id.ToString() : string.Empty,
                             // Dinamik hale getirmek istersen, aşağıdaki satırı kullan:
                             // Subtasks = t.Subtasks?.Select(st => new BoardSubtask(st.Id.ToString(), st.Title, st.IsCompleted)).ToList() ?? new List<BoardSubtask>(),
                             //:Assigned = new Assignee(t.AppUser!.Id.ToString(), t.AppUser.Name, "/images/alice.png")
                         }).ToList()
                )).ToList();
        }

        private List<SelectListItem> CreateSprintSelectList(IEnumerable<SprintDto> sprints)
        {
            return sprints
                .Select(x => new SelectListItem
                {
                    Text = x.SprintName,
                    Value = x.Id.ToString()
                })
                .ToList();
        }
    }

    public class AddSubtaskRequest
    {
        public string TaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
    }
}