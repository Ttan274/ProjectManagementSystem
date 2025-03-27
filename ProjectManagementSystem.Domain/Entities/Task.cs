using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Task : BaseEntity
    {
        public string? TaskName { get; set; }
        public string? TaskDesc { get; set; }
        public bool? Completed { get; set; }

        //Connections
        public Guid SprintId { get; set; }
        public Sprint? Sprint { get; set; }
    }
}
