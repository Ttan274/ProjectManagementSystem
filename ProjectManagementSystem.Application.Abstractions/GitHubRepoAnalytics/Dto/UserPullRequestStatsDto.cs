namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class UserPullRequestStatsDto
    {
        public UserPullRequestStatsDto()
        {

        }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public int PullRequestCount { get; set; }
    }
}
