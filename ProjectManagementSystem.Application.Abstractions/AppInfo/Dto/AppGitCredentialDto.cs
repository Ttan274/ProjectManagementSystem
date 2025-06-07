namespace ProjectManagementSystem.Application.Abstractions.AppInfo.Dto
{
    public class AppGitCredentialDto
    {
        public AppGitCredentialDto()
        {

        }
        public Guid Id { get; set; }
        public int AppCode { get; set; }
        public string? PatToken { get; set; }
        public string? Owner { get; set; }
        public string? RepoName { get; set; }
    }
}
