using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Task;
using ProjectManagementSystem.ViewModel;

namespace ProjectManagementSystem.Controllers
{
    public class ProjectController : Controller
    {
        private readonly IProjectService _projectService;
        private readonly ISprintService _sprintService;
        private readonly ITaskService _taskService;

        public ProjectController(IProjectService projectService, ISprintService sprintService, ITaskService taskService)
        {
            _projectService = projectService;
            _sprintService = sprintService;
            _taskService = taskService;
        }

        [HttpGet]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ProjectMain(Guid id)
        {
            var project = await _projectService.GetProjectById(id);
            ProjectViewModel projectModel = new ProjectViewModel();
            projectModel.Project = project;
            return View(projectModel);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateSprint(ProjectViewModel projectModel)
        {
            if(ModelState.IsValid)
            {
                var response = await _sprintService.CreateSprint(projectModel.SprintToCreate);

                if (response)
                    return RedirectToAction("ProjectMain", "Project");
            }

            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateTask(ProjectViewModel projectModel)
        {
            if (ModelState.IsValid)
            {
                var response = await _taskService.CreateTask(projectModel.TaskToCreate);

                if (response)
                    return RedirectToAction("ProjectMain", "Project");
            }

            return View();
        }
    }
}
