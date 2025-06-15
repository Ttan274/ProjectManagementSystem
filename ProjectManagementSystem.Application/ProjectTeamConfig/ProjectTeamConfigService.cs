using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig;
using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig.Dto;
using ProjectManagementSystem.Application.Abstractions.Repositories.ProjectTeamConfig;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.ProjectTeamConfig
{
    public class ProjectTeamConfigService(
        IServiceResponseHelper serviceResponseHelper,
        IMapper mapper,
        IProjectTeamConfigReadRepository teamConfigReadRepository,
        IProjectTeamConfigWriteRepository teamConfigWriteRepository) : IProjectTeamConfigService
    {
        public async Task<ServiceResponse<ProjectTeamConfigDto>> CreateAsync(ProjectTeamConfigDto projectTeamConfig)
        {
            if (projectTeamConfig == null)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>("Invalid request.");
            }

            projectTeamConfig.Validate(out string validationMessage);

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>(validationMessage);
            }

            try
            {
                var mappedModel = mapper.Map<Domain.Entities.ProjectTeamConfig>(projectTeamConfig);

                await teamConfigWriteRepository.AddAsync(mappedModel);

                return serviceResponseHelper.SetSuccess(projectTeamConfig);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>("Internal server error occured.");
            }
        }

        public async Task<ServiceResponse<ProjectTeamConfigDto?>> GetByIdAsync(Guid id)
        {
            if (id == Guid.Empty)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Invalid request.");
            }

            try
            {
                var projectTeamConfig = teamConfigReadRepository.GetByIdAsync(id, false);

                if (projectTeamConfig == null)
                {
                    return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Not found.");
                }

                var mappedConfig = mapper.Map<ProjectTeamConfigDto>(projectTeamConfig);

                return serviceResponseHelper.SetSuccess(mappedConfig);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Internal server error occured.");
            }
        }

        public async Task<ServiceResponse<ProjectTeamConfigDto?>> GetByProjectIdAsync(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Invalid request.");
            }

            try
            {
                var projectTeamConfig = await teamConfigReadRepository
                    .GetQueryable()
                    .FirstOrDefaultAsync(x => x.ProjectId == projectId);

                if (projectTeamConfig == null)
                {
                    return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Not found.");
                }

                var mappedConfig = mapper.Map<ProjectTeamConfigDto>(projectTeamConfig);

                return serviceResponseHelper.SetSuccess(mappedConfig);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto?>("Internal server error occured.");
            }
        }

        public ServiceResponse<ProjectTeamConfigDto> Update(ProjectTeamConfigDto projectTeamConfig)
        {
            if (projectTeamConfig == null)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>("Invalid request.");
            }

            projectTeamConfig.Validate(out string validationMessage);

            if (!string.IsNullOrWhiteSpace(validationMessage))
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>(validationMessage);
            }

            try
            {
                var mappedModel = mapper.Map<Domain.Entities.ProjectTeamConfig>(projectTeamConfig);

                teamConfigWriteRepository.Update(mappedModel);

                return serviceResponseHelper.SetSuccess(projectTeamConfig);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<ProjectTeamConfigDto>("Internal server error occured.");
            }
        }
    }
}
