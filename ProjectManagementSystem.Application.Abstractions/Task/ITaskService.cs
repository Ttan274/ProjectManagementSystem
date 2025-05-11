using System.Security.Claims;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Task
{
    public interface ITaskService
    {
        Task<bool> CreateTask(TaskDto task);
        Task<List<TaskDto>> GetAllTasksBySprintId(Guid id);
        Task<List<TaskDto>> GetMyAllTasks(Guid userId);
    }
}
