using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig.Dto;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig
{
    public interface IProjectTeamConfigService
    {
        Task<ServiceResponse<ProjectTeamConfigDto>> CreateAsync(ProjectTeamConfigDto projectTeamConfig);
        ServiceResponse<ProjectTeamConfigDto> Update(ProjectTeamConfigDto projectTeamConfig);
        Task<ServiceResponse<ProjectTeamConfigDto?>> GetByIdAsync(Guid id);
        Task<ServiceResponse<ProjectTeamConfigDto?>> GetByProjectIdAsync(Guid projectId);
        Task<ServiceResponse<ProjectConfigSuggestionDto>> GetOllamaSuggestionsAsync(ProjectTeamProfileDto teamProfile);
    }
}
