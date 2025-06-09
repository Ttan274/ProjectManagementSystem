using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Abstractions.AppInfo
{
    public interface IAppInfoService
    {
        Task<ServiceResponse<AppGitCredentialDto>> GetAppGitCredentialAsync(Guid id);
        Task<ServiceResponse<AppInfoDto>> GetByIdAsync(Guid id);
        Task<ServiceResponse<AppInfoDto>> CreateAsync(CreateAppInfoDto createDto);
        Task<ServiceResponse<AppInfoDto>> UpdateAsync(UpdateAppInfoDto updateDto);
        Task<ServiceResponse<bool>> DeleteAsync(Guid id);
        Task<ServiceResponse<List<AppInfoDto>>> GetListAsync();
    }
}
