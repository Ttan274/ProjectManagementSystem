using ProjectManagementSystem.Application.Abstractions.Task.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Task
{
    public interface ITaskService
    {
        Task<bool> CreateTask(TaskDto task);
    }
}
