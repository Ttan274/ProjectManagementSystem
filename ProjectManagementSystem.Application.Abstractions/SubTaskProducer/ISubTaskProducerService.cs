using ProjectManagementSystem.Application.Abstractions.SubTaskProducer.Dto;

namespace ProjectManagementSystem.Application.Abstractions.SubTaskProducer
{
    public interface ISubTaskProducerService
    {
        Task<List<SubTaskItemDto>> GenerateSubTasksAsync(string taskDescription);
    }
}
