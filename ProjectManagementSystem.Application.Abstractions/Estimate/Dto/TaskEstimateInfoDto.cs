using ProjectManagementSystem.Application.Abstractions.Task.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Estimate.Dto
{
    public class TaskEstimateInfoDto
    {
        public Guid EstimateId { get; set; }
        public Guid ProjectId { get; set; }
        public Guid TaskId { get; set; }
        public int Type {  get; set; }
        public List<TaskDto>? Task { get; set; }
        public string Title { get; set; } = string.Empty;
        public string TaskName { get; set; } = string.Empty;
        public string State { get; set; } = string.Empty;
        public string SprintName { get; set; } = string.Empty;
        public int Effort { get; set; }
        public DateTime CreatedDate { get; set; }
    }
}
