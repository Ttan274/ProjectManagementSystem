using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Repositories.Project;
using ProjectManagementSystem.Application.Abstractions.Repositories.Sprint;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Task;

namespace ProjectManagementSystem.Application.Sprint
{
    public class SprintService : ISprintService
    {
        private readonly ISprintReadRepository _sprintReadRepository;
        private readonly ISprintWriteRepository _sprintWriteRepository;
        private readonly IProjectReadRepository _projectReadRepository;
        private readonly ITaskService _taskService;
        private readonly IMapper _mapper;

        public SprintService(ISprintReadRepository sprintReadRepository, ISprintWriteRepository sprintWriteRepository, 
            IProjectReadRepository projectReadRepository, ITaskService taskService, IMapper mapper)
        {
            _sprintReadRepository = sprintReadRepository;
            _sprintWriteRepository = sprintWriteRepository;
            _projectReadRepository = projectReadRepository;
            _taskService = taskService;
            _mapper = mapper;
        }

        public async Task<bool> CreateSprint(SprintDto sprint)
        {
            if (sprint is null)
                return false;

            try
            {
                var project = await _projectReadRepository.GetFirstOrDefaultAsync(x => x.Id == sprint.ProjectId);
                var mappedResult = _mapper.Map<SprintDto, Domain.Entities.Sprint>(sprint);
                mappedResult.Project = project;

                var response = await _sprintWriteRepository.AddAsync(mappedResult);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<SprintDto>> GetAllSprintsByProjectId(Guid id)
        {
            try
            {
                var sprints = await _sprintReadRepository.GetQueryable()
                                                         .Where(x => x.Status)
                                                         .Where(y => y.ProjectId == id)
                                                         .OrderByDescending(x => x.CreatedDatee)
                                                         .ToListAsync();

                if (sprints is null)
                    return [];

                var mappedResult = _mapper.Map<List<Domain.Entities.Sprint>, List<SprintDto>> (sprints);

                foreach (var item in mappedResult)
                {
                    item.Tasks = await _taskService.GetAllTasksBySprintId(item.Id);
                }

                return mappedResult;
            }
            catch (Exception)
            {
                return [];
            }
        }
    }
}
