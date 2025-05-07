using ProjectManagementSystem.Application.Abstractions.Project.Dto;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;

namespace ProjectManagementSystem.ViewModel
{
    public class ProjectViewModel
    {
        public ProjectDto? Project { get; set; }
        public SprintDto? SprintToCreate { get; set; }
        public TaskDto? TaskToCreate { get; set; }
    }
}
