using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Project.Dto;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Task;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.ViewModel;

namespace ProjectManagementSystem.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ISprintService _sprintService;
        private readonly ITaskService _taskService;
        private readonly IUserService _userService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMemoryCache _cache;

        public ProjectController(IProjectService projectService, ISprintService sprintService,
            ITaskService taskService, IUserService userService, UserManager<AppUser> userManager,
            IMemoryCache cache)
        {
            _projectService = projectService;
            _sprintService = sprintService;
            _taskService = taskService;
            _userService = userService;
            _userManager = userManager;
            _cache = cache;
        }

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ProjectMain(Guid id)
        {
            var model = await GetProjectViewModel(id);

            var user = await _userManager.GetUserAsync(User);
            _cache.Set($"{user!.UserName}_MainTitle", model?.Project?.ProjectName, TimeSpan.FromMinutes(30));

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateSprint(Guid projectId)
        {
            var model = new ProjectViewModel { Project = new ProjectDto { Id = projectId } };

            return View("_SprintAction", model);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateSprint(ProjectViewModel projectModel)
        {
            if (ModelState.IsValid)
            {
                var response = await _sprintService.CreateSprint(projectModel.SprintToCreate);

                if (response)
                {
                    var model = await GetProjectViewModel(projectModel.Project.Id);
                    return RedirectToAction("ProjectMain", new { id = projectModel.Project.Id });
                }
            }

            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> TaskAction(Guid projectId, Guid teamId, Guid taskId)
        {
            var model = new ProjectViewModel();

            var project = await _projectService.GetProjectById(projectId);

            var users = await _userService.GetAllUsersByTeamId(project.TeamId);

            if (taskId != Guid.Empty)
                model.TaskToCreate = await _taskService.GetById(taskId);

            model.Project = project;
            model.TaskToCreate ??= new();

            model.TaskToCreate.SprintList = project?.Sprints?.Select(x => new SelectListItem() { Text = x.SprintName, Value = x.Id.ToString() }).ToList();
            model.TaskToCreate.UserList = users.Select(x => new SelectListItem() { Text = x.UserName, Value = x.Id.ToString() }).ToList();

            return View("_TaskAction", model);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> TaskAction(ProjectViewModel projectModel)
        {
            var response = await _taskService.CreateTask(projectModel.TaskToCreate!);

            if (!response)
                return RedirectToAction("Index", "Board", new { projectId = projectModel.Project!.Id });

            var model = await GetProjectViewModel(projectModel.Project!.Id);
            return RedirectToAction("Index", "Board", new { projectId = projectModel.Project.Id });
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> DeleteTask(Guid id)
        {
            var response = await _taskService.Delete(id);

            return Json(new { isSuccess = response });
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetMyTasks()
        {
            var user = await _userManager.GetUserAsync(User);

            var tasks = await _taskService.GetMyAllTasks(user.Id);

            return View(tasks);
        }

        //Utility Method
        private async Task<ProjectViewModel> GetProjectViewModel(Guid id)
        {
            var project = await _projectService.GetProjectById(id);

            var users = await _userService.GetAllUsersByTeamId(project.TeamId);

            ProjectViewModel projectModel = new ProjectViewModel();
            projectModel.Project = project;
            projectModel.TaskToCreate = new()
            {
                SprintList = project.Sprints.Select(x => new SelectListItem() { Text = x.SprintName, Value = x.Id.ToString() }).ToList(),
                UserList = users.Select(x => new SelectListItem() { Text = x.UserName, Value = x.Id.ToString() }).ToList()
            };

            return projectModel;
        }
    }
}
