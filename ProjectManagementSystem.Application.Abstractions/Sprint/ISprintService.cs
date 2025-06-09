using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Abstractions.Sprint
{
    public interface ISprintService
    {
        Task<bool> CreateSprint(SprintDto sprint);
        Task<List<SprintDto>> GetAllSprintsByProjectId(Guid id);
        Task<ServiceResponse<List<SprintDetailsDto>>> GetSprintDetailListAsync(Guid projectId);
        Task<ServiceResponse<List<SprintDto>>> GetListAsync(Guid? projectId = null);
    }
}
