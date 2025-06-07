using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public Comment()
        {
        }
        public string? Header { get; set; }
        public string? Text { get; set; }

        //Connections
        public Guid DocumentationId { get; set; }
        public Documentation? Documentation { get; set; }
    }
}
