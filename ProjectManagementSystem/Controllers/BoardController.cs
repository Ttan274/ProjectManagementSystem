using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Task;
using ProjectManagementSystem.Common.Enums;
using ProjectManagementSystem.Models.Board;

namespace ProjectManagementSystem.Controllers
{
    public class BoardController : Controller
    {
        private readonly ITaskService _taskService;
        private readonly IProjectService _projectService;
        private readonly ISprintService _sprintService;

        public BoardController(ITaskService taskService,
            IProjectService projectService,
            ISprintService sprintService)
        {
            _taskService = taskService;
            _projectService = projectService;
            _sprintService = sprintService;
        }
        public async Task<IActionResult> Index(Guid projectId)
        {
            var model = new ProjectBoardViewModel();
            model.ProjectId = projectId;
            var project = await _projectService.GetProjectById(projectId);
            var sprints = await _sprintService.GetAllSprintsByProjectId(projectId);

            var lastSprint = sprints.FirstOrDefault();

            model.ProjectName = project.ProjectName!;

            if (lastSprint is not null)
            {
                model.SprintId = lastSprint.Id.ToString();

                var tasks = await _taskService.GetAllTasksBySprintId(lastSprint.Id);

                var columns = Enum.GetValues(typeof(ProjecStatus))
                          .Cast<ProjecStatus>()
                          .Select(status => new BoardColumn(
                              ((int)status).ToString(),
                              status.ToString().Replace("InProgress", "In Progress"),
                              tasks.Where(t => t.TaskEffort == status)
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
                                       //t.Subtasks?.Select(st => new BoardSubtask(
                                       //    st.Id.ToString(),
                                       //    st.Title,
                                       //    st.IsCompleted)).ToList() ?? new List<BoardSubtask>(),
                                       Assignees = new List<Assignee>
                                       {
                                           new Assignee(t.AppUser!.Id.ToString(), t.AppUser.Name, "/images/alice.png")
                                       }
                                   }).ToList()
                          )).ToList();

                model.Columns = columns;

            }

            model.SprintList = sprints.Select(x => new SelectListItem { Text = x.SprintName, Value = x.Id.ToString() }).ToList();


            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> BoardRefresh(Guid projectId, Guid sprintId)
        {
            var model = new ProjectBoardViewModel();
            model.ProjectId = projectId;

            var project = await _projectService.GetProjectById(projectId);
            var sprints = await _sprintService.GetAllSprintsByProjectId(projectId);

            model.ProjectName = project.ProjectName!;

            model.SprintId = sprintId.ToString();

            var tasks = await _taskService.GetAllTasksBySprintId(sprintId);

            var columns = Enum.GetValues(typeof(ProjecStatus))
                      .Cast<ProjecStatus>()
                      .Select(status => new BoardColumn(
                          ((int)status).ToString(),
                          status.ToString().Replace("InProgress", "In Progress"),
                          tasks.Where(t => t.TaskEffort == status)
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
                                   //t.Subtasks?.Select(st => new BoardSubtask(
                                   //    st.Id.ToString(),
                                   //    st.Title,
                                   //    st.IsCompleted)).ToList() ?? new List<BoardSubtask>(),
                                   Assignees = new List<Assignee>
                                   {
                                       new Assignee(t.AppUser!.Id.ToString(), t.AppUser.Name, "/images/alice.png")
                                   }
                               }).ToList()
                      )).ToList();

            model.Columns = columns;

            model.SprintList = sprints.Select(x => new SelectListItem { Text = x.SprintName, Value = x.Id.ToString() }).ToList();

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