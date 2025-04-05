using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Project.Dto;
using ProjectManagementSystem.Application.Abstractions.Repositories.Project;

namespace ProjectManagementSystem.Application.Project
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectReadRepository _projectReadRepository;
        private readonly IProjectWriteRepository _projectWriteRepository;
        private readonly IMapper _mapper;

        public ProjectService(IProjectReadRepository projectReadRepository, IProjectWriteRepository projectWriteRepository, IMapper mapper)
        {
            _projectReadRepository = projectReadRepository;
            _projectWriteRepository = projectWriteRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateProject(ProjectDto project)
        {
            if (project is null)
                return false;

            try
            {
                //Team assignment

                var mappedResult = _mapper.Map<ProjectDto, Domain.Entities.Project>(project);

                var response = await _projectWriteRepository.AddAsync(mappedResult);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<ProjectDto>> GetAllProjects()
        {
            try
            {
                var projects = await _projectReadRepository.GetQueryable().Where(x => x.Status).ToListAsync();

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

                var mappedResult = _mapper.Map<Domain.Entities.Project, ProjectDto>(project);

                return mappedResult;
            }
            catch (Exception)
            {
                return new ProjectDto();
            }
        }
    }
}
