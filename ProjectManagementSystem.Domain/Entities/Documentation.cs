using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Documentation : BaseEntity
    {
        public string? Header { get; set; }
        public string? Text { get; set; }
        public int? View { get; set; }

        //Connections
        public Guid? TaskId { get; set; }
        public Task? Task { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
