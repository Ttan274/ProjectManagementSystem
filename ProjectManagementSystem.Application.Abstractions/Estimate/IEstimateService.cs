using ProjectManagementSystem.Application.Abstractions.Estimate.Dto;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Abstractions.Estimate
{
    public interface IEstimateService
    {
        Task<ServiceResponse<EstimateInfoDto>> GetAllByProjectIdAsync(Guid projectId);
        Task<ServiceResponse<bool>> CreateAsync(CreateEstimateDto createDto);
        Task<ServiceResponse<TaskEstimateInfoDto>> GetEstimateTasksInfoAsync(Guid estimateId);
    }
}
