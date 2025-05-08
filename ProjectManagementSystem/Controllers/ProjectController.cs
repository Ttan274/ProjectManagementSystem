using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Sprint;
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

        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> ProjectMain(Guid id)
        {
            var model = await GetProjectViewModel(id);
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> CreateSprint(ProjectViewModel projectModel)
        {
            if(ModelState.IsValid)
            {
                var response = await _sprintService.CreateSprint(projectModel.SprintToCreate);

                if (response)
                {
                    var model = await GetProjectViewModel(projectModel.Project.Id);
                    return View("ProjectMain", model);
                }
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
                {
                    var model = await GetProjectViewModel(projectModel.Project.Id);
                    return View("ProjectMain", model);
                }
            }

            return View();
        }

        //Utility Method
        private async Task<ProjectViewModel> GetProjectViewModel(Guid id)
        {
            var project = await _projectService.GetProjectById(id);
            var sprints = project.Sprints.Select(x => new SelectListItem() { Text = x.SprintName, Value = x.Id.ToString() });

            ProjectViewModel projectModel = new ProjectViewModel();
            projectModel.Project = project;
            projectModel.TaskToCreate = new()
            {
                SprintList = sprints.ToList(),
            };

            return projectModel;
        }
    }
}
