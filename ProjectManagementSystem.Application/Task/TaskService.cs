using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Dto;
using ProjectManagementSystem.Application.Abstractions.Repositories.Sprint;
using ProjectManagementSystem.Application.Abstractions.Repositories.Task;
using ProjectManagementSystem.Application.Abstractions.Task;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;
using ProjectManagementSystem.Common.Enums;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Task
{
    public class TaskService : ITaskService
    {
        private readonly ITaskReadRepository _taskReadRepository;
        private readonly ITaskWriteRepository _taskWriteRepository;
        private readonly ISprintReadRepository _sprintReadRepository;
        private readonly IMapper _mapper;
        private readonly IServiceResponseHelper _serviceResponseHelper;

        public TaskService(ITaskReadRepository taskReadRepository, ITaskWriteRepository taskWriteRepository,
            ISprintReadRepository sprintReadRepository, IMapper mapper, IServiceResponseHelper serviceResponseHelper)
        {
            _taskReadRepository = taskReadRepository;
            _taskWriteRepository = taskWriteRepository;
            _sprintReadRepository = sprintReadRepository;
            _mapper = mapper;
            _serviceResponseHelper = serviceResponseHelper;
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

                bool response;

                if (task.Id == Guid.Empty)
                    response = await _taskWriteRepository.AddAsync(mappedResult);
                else
                    response = _taskWriteRepository.Update(mappedResult);

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
                                                 .Include(x => x.Documentation)
                                                 .OrderByDescending(x => x.CreatedDatee)
                                                 .ToListAsync();
            if (tasks is null)
                return [];

            var mappedResult = _mapper.Map<List<Domain.Entities.Task>, List<TaskDto>>(tasks);

            return mappedResult;
        }

        public async Task<bool> UpdateTaskStatus(string id, string status)
        {
            try
            {
                int stat = Convert.ToInt32(status);

                var enumStat = (ProjectStatus)stat;

                var task = await _taskReadRepository.GetFirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

                task.State = enumStat;

                var response = _taskWriteRepository.Update(task);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateTaskUser(string id, string userId)
        {
            try
            {
                var task = await _taskReadRepository.GetFirstOrDefaultAsync(x => x.Id == Guid.Parse(id));

                task.UserId = Guid.Parse(userId);

                var response = _taskWriteRepository.Update(task);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<TaskDto> GetById(Guid id)
        {
            try
            {
                var task = await _taskReadRepository.GetQueryable(tracking: false)
                                                    .Include(x => x.DependentTasks)
                                                    .FirstOrDefaultAsync(x => x.Id == id);

                var mappedResult = _mapper.Map<Domain.Entities.Task?, TaskDto>(task);

                return mappedResult;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> Delete(Guid id)
        {
            if (id == Guid.Empty)
                return false;

            try
            {
                return await _taskWriteRepository.RemoveAsync(id);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Guid> CreateSubtask(CreateSubTaskDto dto)
        {
            try
            {
                var task = await _taskReadRepository.GetQueryable()
                                                    .Include(x => x.DependentTasks)
                                                    .FirstOrDefaultAsync(x => x.Id == dto.ParentTaskId);

                if (task == null)
                    throw new Exception();

                var entity = new Domain.Entities.Task
                {
                    UserId = task.UserId,
                    TaskId = dto.ParentTaskId,
                    SprintId = task.SprintId,
                    TaskName = dto.Title,
                    TaskDesc = dto.Description,
                    State = task.State
                };

                task.DependentTasks ??= [];

                task.DependentTasks.Add(entity);

                await _taskWriteRepository.SaveAsync();

                return entity.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> DeleteSubtask(DeleteSubTaskDto dto)
        {
            try
            {
                return await _taskWriteRepository.RemoveAsync(dto.Id);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> UpdateSubtask(UpdateSubTaskDto dto)
        {
            try
            {
                var task = await _taskReadRepository.GetFirstOrDefaultAsync(x => x.Id == dto.TaskId);

                task.TaskName = dto.Title;
                task.TaskDesc = dto.Description;

                return _taskWriteRepository.Update(task);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<ServiceResponse<TaskDto>> GetEstimateTask(Guid id)
        {
            try
            {
                var task = await _taskReadRepository.GetQueryable()
                                                        .Include(x => x.AppUser)
                                                        .Include(x => x.DependentTasks)
                                                     .Where(x => x.Status)
                                                     .Where(y => y.Id == id)
                                                     .OrderByDescending(x => x.CreatedDatee)
                                                     .FirstOrDefaultAsync();

                task ??= new Domain.Entities.Task();

                var mappedResult = _mapper.Map<Domain.Entities.Task, TaskDto>(task);

                return _serviceResponseHelper.SetSuccess(mappedResult);
            }
            catch (Exception)
            {
                return _serviceResponseHelper.SetError<TaskDto>("Server Error");
            }
        }

        public async Task<ServiceResponse<bool>> UpdateTaskEffort(Guid taskId, int effort)
        {
            try
            {
                var task = await _taskReadRepository.GetFirstOrDefaultAsync(x => x.Id == taskId);

                task.EffortScore = effort;

                return _serviceResponseHelper.SetSuccess(_taskWriteRepository.Update(task));
            }
            catch (Exception)
            {
                return _serviceResponseHelper.SetError<bool>("Server Error");
            }
        }
    }
}
