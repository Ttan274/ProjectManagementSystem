using ProjectManagementSystem.Application.Abstractions.Project.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Project
{
    public interface IProjectService
    {
        Task<bool> CreateProject(ProjectDto project);
        Task<ProjectDto> GetProjectById(Guid id);
        Task<List<ProjectDto>> GetAllProjects();
    }
}
