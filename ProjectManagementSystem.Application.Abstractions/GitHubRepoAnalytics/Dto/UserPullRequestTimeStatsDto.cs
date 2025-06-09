namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class UserPullRequestTimeStatsDto
    {
        public UserPullRequestTimeStatsDto()
        {
            PullRequestTimes = [];
            PullRequests = [];
        }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public List<PullRequestRecord> PullRequests { get; set; }
        public List<TimeSpan> PullRequestTimes { get; set; }

        public TimeSpan AverageTimeOpen =>
            PullRequestTimes != null && PullRequestTimes.Count > 0
                ? TimeSpan.FromTicks((long)PullRequestTimes.Average(t => t.Ticks))
                : TimeSpan.Zero;
    }
}
