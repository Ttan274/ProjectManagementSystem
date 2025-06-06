using ProjectManagementSystem.Common.Enums;
using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Project : BaseEntity
    {
        public Project()
        {
            Sprints = new HashSet<Sprint>();
        }
        public string? ProjectName { get; set; }
        public string? ProjectDesc { get; set; }
        public string? ProjectNumber { get; set; }
        public int? EstimatedHours { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public Priority? Priority { get; set; }

        //Connections
        public Guid TeamId { get; set; }
        public Team? Team { get; set; }
        public ICollection<Sprint>? Sprints { get; set; }
    }
}
