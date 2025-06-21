using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Estimate : BaseEntity
    {
        public string? Title { get; set; }
        public int Type { get; set; }

        public Guid SprintId { get; set; }
        public Sprint? Sprint { get; set; }

        public Guid ProjectId { get; set; }
        public Project? Proje { get; set; }
    }
}
