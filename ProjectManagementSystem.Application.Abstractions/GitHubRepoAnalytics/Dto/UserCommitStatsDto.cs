namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class UserCommitStatsDto
    {
        public UserCommitStatsDto()
        {

        }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public int CommitCount { get; set; }
        public int TotalAdditions { get; set; }
        public int TotalDeletions { get; set; }
        public int NetChanges => TotalAdditions - TotalDeletions;
    }
}
