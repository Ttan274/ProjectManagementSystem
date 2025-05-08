using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Project.Dto;
using ProjectManagementSystem.Application.Abstractions.Repositories.Project;
using ProjectManagementSystem.Application.Abstractions.Repositories.Team;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Team;

namespace ProjectManagementSystem.Application.Project
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectReadRepository _projectReadRepository;
        private readonly IProjectWriteRepository _projectWriteRepository;
        private readonly ITeamReadRepository _teamReadRepository;
        private readonly ISprintService _sprintService;
        private readonly IMapper _mapper;

        public ProjectService(IProjectReadRepository projectReadRepository, IProjectWriteRepository projectWriteRepository, 
            ITeamReadRepository teamReadRepository, ISprintService sprintService, IMapper mapper)
        {
            _projectReadRepository = projectReadRepository;
            _projectWriteRepository = projectWriteRepository;
            _teamReadRepository = teamReadRepository;
            _sprintService = sprintService;
            _mapper = mapper;
        }

        public async Task<bool> CreateProject(ProjectDto project)
        {
            if (project is null)
                return false;

            try
            {
                var dept = await _teamReadRepository.GetFirstOrDefaultAsync(x => x.Id == project.TeamId);
                var mappedResult = _mapper.Map<ProjectDto, Domain.Entities.Project>(project);
                mappedResult.Team = dept;

                var response = await _projectWriteRepository.AddAsync(mappedResult);

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
                var projects = await _projectReadRepository.GetQueryable().Where(x => x.Status).Where(y => y.TeamId == id).ToListAsync();

                if (projects is null)
                    return [];

                var mappedResult = _mapper.Map<List<Domain.Entities.Project>, List<ProjectDto>>(projects);

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
                var project = await _projectReadRepository.GetByIdAsync(id);
                var sprints = await _sprintService.GetAllSprintsByProjectId(id);
                //project.Team = await _teamReadRepository.GetByIdAsync(project.TeamId);

                var mappedResult = _mapper.Map<Domain.Entities.Project, ProjectDto>(project);
                mappedResult.Sprints = sprints;

                return mappedResult;
            }
            catch (Exception)
            {
                return new ProjectDto();
            }
        }
    }
}
