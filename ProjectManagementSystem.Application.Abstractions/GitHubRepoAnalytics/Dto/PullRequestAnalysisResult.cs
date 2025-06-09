namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class PullRequestAnalysisResult
    {
        public PullRequestAnalysisResult()
        {
            FastestClosedPullRequests = [];
            SlowestClosedPullRequests = [];
            StaleOpenPullRequests = [];
            MostCommentedPullRequests = [];
            PullRequestsWithoutComments = [];
            OpenPullRequests = [];
            MergedPullRequests = [];
            PullRequestsPerWeek = [];
            MostActiveWeeks = [];
            LeastActiveWeeks = [];
        }

        public TimeSpan? AverageDuration { get; set; }
        public double AverageCommentCount { get; set; }

        public List<PullRequestRecord> FastestClosedPullRequests { get; set; }
        public List<PullRequestRecord> SlowestClosedPullRequests { get; set; }
        public List<PullRequestRecord> StaleOpenPullRequests { get; set; }

        public List<PullRequestRecord> MostCommentedPullRequests { get; set; }
        public List<PullRequestRecord> PullRequestsWithoutComments { get; set; }

        public int TotalOpenCount { get; set; }
        public int TotalClosedCount { get; set; }
        public int TotalMergedCount { get; set; }

        public List<PullRequestRecord> OpenPullRequests { get; set; }
        public List<PullRequestRecord> MergedPullRequests { get; set; }

        public Dictionary<int, int> PullRequestsPerWeek { get; set; }
        public List<int> MostActiveWeeks { get; set; }
        public List<int> LeastActiveWeeks { get; set; }
    }
}
