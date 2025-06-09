namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class PullRequestRecord
    {
        public PullRequestRecord()
        {

        }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public int Number { get; set; }
        public string? Title { get; set; }
        public string? Url { get; set; }
        public string? State { get; set; }
        public bool Merged { get; set; }
        public int Comments { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ClosedAt { get; set; }
        public DateTime? MergedAt { get; set; }
        public TimeSpan? Duration => ClosedAt.HasValue ? ClosedAt.Value - CreatedAt : null;
    }
}
