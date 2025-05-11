using ProjectManagementSystem.Common.Enums;

namespace ProjectManagementSystem.Application.Abstractions.Task.Dto
{
    public class TaskDetailDto
    {
        public string? TaskName { get; set; }
        public string? TaskDesc { get; set; }
        public string? TaskNumber { get; set; }
        public Priority? Priority { get; set; }
        public ProjecStatus? TaskEffort { get; set; }
        public bool? IsUrgent { get; set; }
        public bool? Completed { get; set; }
    }
}
