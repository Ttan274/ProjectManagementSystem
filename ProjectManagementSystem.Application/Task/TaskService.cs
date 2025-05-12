using System.Security.Claims;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Repositories.Sprint;
using ProjectManagementSystem.Application.Abstractions.Repositories.Task;
using ProjectManagementSystem.Application.Abstractions.Task;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;

namespace ProjectManagementSystem.Application.Task
{
    public class TaskService : ITaskService
    {
        private readonly ITaskReadRepository _taskReadRepository;
        private readonly ITaskWriteRepository _taskWriteRepository;
        private readonly ISprintReadRepository _sprintReadRepository;
        private readonly IMapper _mapper;

        public TaskService(ITaskReadRepository taskReadRepository, ITaskWriteRepository taskWriteRepository,
            ISprintReadRepository sprintReadRepository, IMapper mapper)
        {
            _taskReadRepository = taskReadRepository;
            _taskWriteRepository = taskWriteRepository;
            _sprintReadRepository = sprintReadRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateTask(TaskDto task)
        {
            if (task is null)
                return false;

            try
            {
                var sprint = await _sprintReadRepository.GetFirstOrDefaultAsync(x => x.Id == Guid.Parse(task.SprintId));
                var mappedResult = _mapper.Map<TaskDto, Domain.Entities.Task>(task);
                mappedResult.Sprint = sprint;

                var response = await _taskWriteRepository.AddAsync(mappedResult);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<TaskDto>> GetAllTasksBySprintId(Guid id)
        {
            try
            {
                var tasks = await _taskReadRepository.GetQueryable()
                                                     .Include(x => x.AppUser)
                                                     .Where(x => x.Status)
                                                     .Where(y => y.SprintId == id)
                                                     .OrderByDescending(x => x.CreatedDatee).ToListAsync();

                if (tasks is null)
                    return [];

                var mappedResult = _mapper.Map<List<Domain.Entities.Task>, List<TaskDto>>(tasks);

                return mappedResult;
            }
            catch (Exception)
            {
                return [];
            }
        }
        public async Task<List<TaskDto>> GetMyAllTasks(Guid userId)
        {
            var tasks = await _taskReadRepository.GetQueryable()
                                                 .Where(x => x.UserId == userId && x.Status)
                                                 .Include(x => x.AppUser)
                                                 .OrderByDescending(x => x.CreatedDatee)
                                                 .ToListAsync();
            if (tasks is null)
                return [];

            var mappedResult = _mapper.Map<List<Domain.Entities.Task>, List<TaskDto>>(tasks);

            return mappedResult;
        }
    }
}
