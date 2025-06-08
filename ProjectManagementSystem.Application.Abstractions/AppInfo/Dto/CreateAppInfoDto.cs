namespace ProjectManagementSystem.Application.Abstractions.AppInfo.Dto
{
    public class CreateAppInfoDto
    {
        public CreateAppInfoDto()
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

        public void Validate(out string validationResult)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(Name))
                errors.Add("Name is required.");

            if (AppCode <= 0)
                errors.Add("AppCode must be greater than zero.");

            if (string.IsNullOrWhiteSpace(GitHubPatToken))
                errors.Add("GitHub PAT token is required.");

            if (string.IsNullOrWhiteSpace(GitHubOwner))
                errors.Add("GitHub owner is required.");

            if (string.IsNullOrWhiteSpace(GitHubRepo))
                errors.Add("GitHub repository name is required.");

            if (ProjectId == null || ProjectId == Guid.Empty)
                errors.Add("Project ID is required.");

            validationResult = string.Join(" ", errors);
        }
    }
}
