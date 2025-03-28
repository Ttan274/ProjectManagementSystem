using Org.BouncyCastle.Asn1.Mozilla;
using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Task : BaseEntity
    {
        public string? TaskName { get; set; }
        public string? TaskDesc { get; set; }
        public string? TaskNumber { get; set; }
        public int? Priority { get; set; }
        public int? TaskEffort { get; set; }
        public bool? IsUrgent { get; set; }
        public bool? Completed { get; set; }

        //Connections
        public Guid SprintId { get; set; }
        public Sprint? Sprint { get; set; }
        public Guid UserId { get; set; }
        public AppUser? AppUser { get; set; }
        public ICollection<Task>? DependentTasks { get; set; }
    }
}
