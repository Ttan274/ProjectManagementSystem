using ProjectManagementSystem.Common.Enums;
using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Task : BaseEntity
    {
        public Task()
        {
            DependentTasks = new HashSet<Task>();
        }
        public string? TaskName { get; set; }
        public string? TaskDesc { get; set; }
        public string? TaskNumber { get; set; }
        public Priority? Priority { get; set; }
        public ProjectStatus? State { get; set; }
        public TaskType Type { get; set; }
        public int EffortScore { get; set; }
        public bool? IsUrgent { get; set; }
        public bool? Completed { get; set; }
        public DateTime? CompletedAt { get; set; }

        //Connections
        public Guid SprintId { get; set; }
        public Sprint? Sprint { get; set; }
        public Guid UserId { get; set; }
        public AppUser? AppUser { get; set; }
        public Guid? DocumentationId { get; set; }
        public Documentation? Documentation { get; set; }
        public ICollection<Task>? DependentTasks { get; set; }
    }
}
