using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Project.Dto;
using ProjectManagementSystem.Application.Abstractions.Repositories.Project;
using ProjectManagementSystem.Application.Abstractions.Repositories.Team;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Project
{
    public class ProjectService(
        IServiceResponseHelper serviceResponseHelper,
        IProjectReadRepository projectReadRepository,
        IProjectWriteRepository projectWriteRepository,
        ITeamReadRepository teamReadRepository,
        ISprintService sprintService,
        IMapper mapper) : IProjectService
    {
        public async Task<bool> CreateProject(ProjectDto project)
        {
            if (project is null)
                return false;

            try
            {
                var dept = await teamReadRepository.GetFirstOrDefaultAsync(x => x.Id == project.TeamId);
                var mappedResult = mapper.Map<ProjectDto, Domain.Entities.Project>(project);
                mappedResult.Team = dept;

                var response = await projectWriteRepository.AddAsync(mappedResult);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<ProjectDto>> GetAllProjectsByTeamId(Guid id)
        {
            try
            {
                var projects = await projectReadRepository.GetQueryable().Where(x => x.Status).Where(y => y.TeamId == id).ToListAsync();

                if (projects is null)
                    return [];

                var mappedResult = mapper.Map<List<Domain.Entities.Project>, List<ProjectDto>>(projects);

                return mappedResult;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<ProjectDto> GetProjectById(Guid id)
        {
            if (id == Guid.Empty)
                return new ProjectDto();

            try
            {
                var project = await projectReadRepository.GetByIdAsync(id);
                var sprints = await sprintService.GetAllSprintsByProjectId(id);
                //project.Team = await _teamReadRepository.GetByIdAsync(project.TeamId);

                var mappedResult = mapper.Map<Domain.Entities.Project, ProjectDto>(project);
                mappedResult.Sprints = sprints;

                return mappedResult;
            }
            catch (Exception)
            {
                return new ProjectDto();
            }
        }

        public async Task<ServiceResponse<AppProjectInfoDto>> GetAppsByProjectAsync(Guid projectId)
        {
            if (projectId == Guid.Empty)
            {
                return serviceResponseHelper.SetError<AppProjectInfoDto>("Invalid request.");
            }

            try
            {
                var result = await projectReadRepository
                    .GetQueryable(tracking: false)
                    .Where(x => x.Id == projectId)
                    .Include(x => x.Applications.Where(x => x.IsDeleted == 0))
                    .Include(x => x.Team)
                    .FirstOrDefaultAsync();

                if (result == null)
                {
                    return serviceResponseHelper.SetError<AppProjectInfoDto>("Project not found.");
                }

                var appProjectInfo = new AppProjectInfoDto()
                {
                    Project = mapper.Map<ProjectDto>(result),
                    AppInfos = mapper.Map<List<AppInfoDto>>(result.Applications)
                };

                return serviceResponseHelper.SetSuccess(appProjectInfo);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError<AppProjectInfoDto>("Internal server error occured.");
            }
        }
    }
}
