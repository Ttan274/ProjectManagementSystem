using ProjectManagementSystem.Common.Consts;
using ProjectManagementSystem.Common.Enums;

namespace ProjectManagementSystem.Application.Abstractions.Task.Dto
{
    public class TaskSummaryDto
    {
        public TaskSummaryDto()
        {
            AssignedMember = Defaults.UNKNOWN_USER;
        }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? Number { get; set; }
        public Priority? Priority { get; set; }
        public TaskType Type { get; set; }
        public int? EffortScore { get; set; }
        public bool? IsCompleted { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? AssignedMember { get; set; }
        public bool HasDocumentation { get; set; }
    }
}
