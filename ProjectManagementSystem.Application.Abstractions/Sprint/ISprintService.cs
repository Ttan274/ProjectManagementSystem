using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Sprint
{
    public interface ISprintService
    {
        Task<bool> CreateSprint(SprintDto sprint);
        Task<List<SprintDto>> GetAllSprintsByProjectId(Guid id);
    }
}
