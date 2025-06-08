using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.AppInfo;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.Repositories.AppInfo;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.AppInfo
{
    public class AppInfoService(
        IMapper mapper,
        IAppInfoReadRepository appInfoReadRepository,
        IAppInfoWriteRepository appInfoWriteRepository,
        IServiceResponseHelper serviceResponseHelper)
        : IAppInfoService
    {
        public async Task<ServiceResponse<AppInfoDto>> CreateAsync(CreateAppInfoDto createDto)
        {
            if (createDto == null)
            {
                return serviceResponseHelper.SetError<AppInfoDto>("Invalid request.");
            }

            createDto.Validate(out string validationResult);

            if (!string.IsNullOrWhiteSpace(validationResult))
            {
                return serviceResponseHelper.SetError<AppInfoDto>(validationResult);
            }

            try
            {
                var appCodeExists = await appInfoReadRepository
                    .GetQueryable()
                    .AnyAsync(x => x.AppCode == createDto.AppCode);

                if (appCodeExists)
                {
                    return serviceResponseHelper.SetError<AppInfoDto>("This app code already exists.");
                }

                var mappedAppInfo = mapper.Map<Domain.Entities.AppInfo>(createDto);

                await appInfoWriteRepository.AddAsync(mappedAppInfo);

                var appInfo = await appInfoReadRepository.GetFirstOrDefaultAsync(app => app.Id == mappedAppInfo.Id);

                if (appInfo == null)
                {
                    return serviceResponseHelper.SetError<AppInfoDto>("Create operation failed.");
                }

                return serviceResponseHelper.SetSuccess(mapper.Map<AppInfoDto>(appInfo));
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<AppInfoDto>("Internal server error");
            }
        }

        public async Task<ServiceResponse<bool>> DeleteAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return serviceResponseHelper.SetError(false, "Invalid request");
            }

            try
            {
                var appInfo = await appInfoReadRepository.GetFirstOrDefaultAsync(x => x.Id == id);

                if (appInfo == null)
                {
                    return serviceResponseHelper.SetError(false, "App info not found");
                }

                appInfo.IsDeleted = 1;

                appInfoWriteRepository.Update(appInfo);

                return serviceResponseHelper.SetSuccess(true);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError(false, "Internal server error occured.");
            }
        }

        public async Task<ServiceResponse<AppGitCredentialDto>> GetAppGitCredentialAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return serviceResponseHelper.SetError<AppGitCredentialDto>("Invalid request.");
            }

            try
            {
                var appInfo = await appInfoReadRepository.GetFirstOrDefaultAsync(method: q => q.Id == id);

                if (appInfo == null)
                {
                    return serviceResponseHelper.SetError<AppGitCredentialDto>("App info not found.");
                }

                var mappedAppInfo = mapper.Map<AppGitCredentialDto>(appInfo);

                return serviceResponseHelper.SetSuccess(mappedAppInfo);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<AppGitCredentialDto>("Internal server error occured.");
            }
        }

        public async Task<ServiceResponse<AppInfoDto>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return serviceResponseHelper.SetError<AppInfoDto>("Invalid request.");
            }

            try
            {
                var appInfo = await appInfoReadRepository.GetFirstOrDefaultAsync(method: q => q.Id == id && q.IsDeleted == 0);

                if (appInfo == null)
                {
                    return serviceResponseHelper.SetError<AppInfoDto>("App info not found.");
                }

                var mappedAppInfo = mapper.Map<AppInfoDto>(appInfo);

                return serviceResponseHelper.SetSuccess(mappedAppInfo);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<AppInfoDto>("Internal server error occured.");
            }
        }

        public async Task<ServiceResponse<AppInfoDto>> UpdateAsync(UpdateAppInfoDto updateDto)
        {
            if (updateDto == null || updateDto.Id == Guid.Empty)
            {
                return serviceResponseHelper.SetError<AppInfoDto>("Invalid request.");
            }

            updateDto.Validate(out string validationResult);

            if (!string.IsNullOrWhiteSpace(validationResult))
            {
                return serviceResponseHelper.SetError<AppInfoDto>(validationResult);
            }

            try
            {
                var appInfo = await appInfoReadRepository.GetFirstOrDefaultAsync(method: q => q.Id == updateDto.Id);

                if (appInfo == null)
                {
                    return serviceResponseHelper.SetError<AppInfoDto>("App info not found.");
                }

                var mappedAppInfo = mapper.Map(updateDto, appInfo);

                appInfoWriteRepository.Update(appInfo);

                return serviceResponseHelper.SetSuccess(mapper.Map<AppInfoDto>(appInfo));
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<AppInfoDto>("Internal server error occured.");
            }
        }
    }
}
