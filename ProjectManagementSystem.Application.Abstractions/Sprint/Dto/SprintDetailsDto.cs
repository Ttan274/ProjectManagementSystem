using ProjectManagementSystem.Application.Abstractions.Task.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Sprint.Dto
{
    public class SprintDetailsDto
    {
        public SprintDetailsDto()
        {
            Tasks = [];
        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public Guid? ProjectId { get; set; }
        public List<TaskSummaryDto> Tasks { get; set; }
    }
}
