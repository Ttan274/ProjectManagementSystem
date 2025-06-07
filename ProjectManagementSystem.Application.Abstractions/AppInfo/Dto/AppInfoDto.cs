namespace ProjectManagementSystem.Application.Abstractions.AppInfo.Dto
{
    public class AppInfoDto
    {
        public AppInfoDto()
        {

        }
        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }
        public int AppCode { get; set; }
        public string? GitHubPatToken { get; set; }
        public string? GitHubOwner { get; set; }
        public string? GitHubRepo { get; set; }
        public DateTime? DecommissionDate { get; set; }
        public Guid? ProjectId { get; set; }
    }
}
