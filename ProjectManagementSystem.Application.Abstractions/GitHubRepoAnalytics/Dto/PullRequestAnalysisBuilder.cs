namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class PullRequestAnalysisBuilder
    {
        private readonly List<PullRequestRecord> pullRequests;
        private readonly PullRequestAnalysisResult result;

        public PullRequestAnalysisBuilder(List<PullRequestRecord> pullRequests)
        {
            this.pullRequests = pullRequests;
            result = new PullRequestAnalysisResult();
        }

        public PullRequestAnalysisBuilder WithFastestClosedPullRequests(TimeSpan threshold)
        {
            result.FastestClosedPullRequests =
                [.. pullRequests.Where(pr => pr.Duration.HasValue && pr.Duration.Value <= threshold)];

            return this;
        }

        public PullRequestAnalysisBuilder WithSlowestClosedPullRequests(TimeSpan threshold)
        {
            result.SlowestClosedPullRequests =
                [.. pullRequests.Where(pr => pr.Duration.HasValue && pr.Duration.Value > threshold)];

            return this;
        }

        public PullRequestAnalysisBuilder WithStaleOpenPullRequests(TimeSpan threshold)
        {
            result.StaleOpenPullRequests =
                [.. pullRequests.Where(pr => !pr.ClosedAt.HasValue && (DateTime.Now - pr.CreatedAt) > threshold)];

            return this;
        }

        public PullRequestAnalysisBuilder WithMostCommentedPullRequests(int minComments)
        {
            result.MostCommentedPullRequests = [.. pullRequests.Where(pr => pr.Comments >= minComments)];

            return this;
        }

        public PullRequestAnalysisBuilder WithAverageCommentCount()
        {
            result.AverageCommentCount = pullRequests.Average(pr => pr.Comments);
            return this;
        }

        public PullRequestAnalysisBuilder WithPullRequestsWithoutComments()
        {
            result.PullRequestsWithoutComments = [.. pullRequests.Where(pr => pr.Comments == 0)];

            return this;
        }

        public PullRequestAnalysisBuilder WithTotalOpenCount()
        {
            result.TotalOpenCount = pullRequests.Count(pr => !pr.ClosedAt.HasValue);

            return this;
        }

        public PullRequestAnalysisBuilder WithTotalClosedCount()
        {
            result.TotalClosedCount = pullRequests.Count(pr => pr.ClosedAt.HasValue);

            return this;
        }

        public PullRequestAnalysisBuilder WithTotalMergedCount()
        {
            result.TotalMergedCount = pullRequests.Count(pr => pr.Merged);

            return this;
        }

        public PullRequestAnalysisBuilder WithOpenPullRequests()
        {
            result.OpenPullRequests = [.. pullRequests.Where(pr => !pr.ClosedAt.HasValue)];

            return this;
        }

        public PullRequestAnalysisBuilder WithMergedPullRequests()
        {
            result.MergedPullRequests = [.. pullRequests.Where(pr => pr.Merged)];

            return this;
        }

        public PullRequestAnalysisBuilder WithPullRequestsPerWeek()
        {
            result.PullRequestsPerWeek = pullRequests
                .GroupBy(pr => GetIsoWeekNumber(pr.CreatedAt))
                .ToDictionary(g => g.Key, g => g.Count());
            return this;
        }

        public PullRequestAnalysisBuilder WithMostActiveWeeks()
        {
            var mostActiveWeeks = result.PullRequestsPerWeek
                .Where(kv => kv.Value == result.PullRequestsPerWeek.Values.Max())
                .Select(kv => kv.Key)
                .ToList();
            result.MostActiveWeeks = mostActiveWeeks;
            return this;
        }

        public PullRequestAnalysisBuilder WithLeastActiveWeeks()
        {
            var leastActiveWeeks = result.PullRequestsPerWeek
                .Where(kv => kv.Value == result.PullRequestsPerWeek.Values.Min())
                .Select(kv => kv.Key)
                .ToList();
            result.LeastActiveWeeks = leastActiveWeeks;
            return this;
        }

        public PullRequestAnalysisResult Build()
        {
            return result;
        }

        private static int GetIsoWeekNumber(DateTime date)
        {
            var d = date.AddDays(3 - ((int)date.DayOfWeek + 6) % 7);
            var cal = System.Globalization.CultureInfo.InvariantCulture.Calendar;
            var weekNum = cal.GetWeekOfYear(d, System.Globalization.CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
            return weekNum;
        }
    }
}
