using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Project : BaseEntity
    {
        public string? ProjectName { get; set; }
        public string? ProjectDesc { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public bool? Completed { get; set; }

        //Connections
        public Guid TeamId { get; set; }
        public Team? Team { get; set; }
        public ICollection<Sprint>? Sprints { get; set; }
    }
}
