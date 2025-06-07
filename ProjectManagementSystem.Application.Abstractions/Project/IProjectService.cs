using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.Project.Dto;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Abstractions.Project
{
    public interface IProjectService
    {
        Task<bool> CreateProject(ProjectDto project);
        Task<ProjectDto> GetProjectById(Guid id);
        Task<List<ProjectDto>> GetAllProjectsByTeamId(Guid id);
        Task<ServiceResponse<AppProjectInfoDto>> GetAppsByProjectAsync(Guid projectId);
    }
}
