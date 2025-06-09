using ProjectManagementSystem.Common.Extensions;

namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class CommitSizeAnalysis : ICommitAnalysis
    {
        public CommitSizeAnalysis()
        {
            AnalysisName = nameof(CommitSizeAnalysis);
            DailyCommits = [];
            WeeklyCommits = [];
            MonthlyCommits = [];
        }
        public CommitSizeAnalysis(string analysisName)
        {
            DailyCommits = [];
            WeeklyCommits = [];
            MonthlyCommits = [];
            AnalysisName = analysisName;
        }
        public string? AnalysisName { get; set; }
        public int TotalCommits { get; private set; }
        public double AvgCommitSize { get; private set; }
        public Dictionary<string, int> DailyCommits { get; private set; }
        public Dictionary<string, int> WeeklyCommits { get; private set; }
        public Dictionary<string, int> MonthlyCommits { get; private set; }

        public void Analyze(List<GitCommitDto> commits)
        {
            TotalCommits = commits.Count;

            AvgCommitSize = commits.Average(c => c.Additions + c.Deletions);

            DailyCommits = commits
                .GroupBy(c => c.Date?.ToString("yyyy-MM-dd"))
                .ToDictionary(g => g.Key ?? DateTime.MinValue.ToString("yyyy-MM-dd"), g => g.Count());

            WeeklyCommits = commits
                .GroupBy(c => $"{c.Date?.Year}-W{c.Date?.GetWeekOfYear()}")
                .ToDictionary(g => g.Key, g => g.Count());

            MonthlyCommits = commits
                .GroupBy(c => c.Date?.ToString("yyyy-MM"))
                .ToDictionary(g => g.Key ?? DateTime.MinValue.ToString("yyyy-MM-dd"), g => g.Count());
        }
    }
}
