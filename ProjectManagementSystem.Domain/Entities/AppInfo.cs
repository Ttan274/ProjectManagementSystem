using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class AppInfo : BaseEntity
    {
        public AppInfo()
        {

        }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int AppCode { get; set; }
        public string? GitHubPatToken { get; set; }
        public string? GitHubOwner { get; set; }
        public string? GitHubRepo { get; set; }
        public DateTime? DecommissionDate { get; set; }
        public Guid? ProjectId { get; set; }
        public Project? Project { get; set; }
        public int IsDeleted { get; set; }
    }
}
