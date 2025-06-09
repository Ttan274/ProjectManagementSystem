namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class ProjectHealthAnalysis : ICommitAnalysis
    {
        public ProjectHealthAnalysis()
        {
            ActivityTrend = [];
            FileChangeFrequency = [];
            AnalysisName = nameof(ProjectHealthAnalysis);
        }
        public ProjectHealthAnalysis(string projectName)
        {
            ActivityTrend = [];
            FileChangeFrequency = [];
            AnalysisName = projectName + " Project Health";
        }
        public string AnalysisName { get; set; }
        public Dictionary<string, int> ActivityTrend { get; private set; }
        public Dictionary<string, int> FileChangeFrequency { get; private set; }
        public int NetGrowth { get; private set; }
        public double CommitVelocity { get; private set; }

        public void Analyze(List<GitCommitDto> commits)
        {
            var sixMonthsAgo = DateTime.Now.AddMonths(-6);
            ActivityTrend = commits
                .Where(c => c.Date >= sixMonthsAgo)
                .GroupBy(c => c.Date?.ToString("yyyy-MM"))
                .OrderBy(g => g.Key)
                .ToDictionary(g => g.Key ?? DateTime.MinValue.ToString("yyyy-MM-dd"), g => g.Count());

            FileChangeFrequency = commits
                .SelectMany(c => c.FilesChanged)
                .GroupBy(file => file)
                .ToDictionary(g => g.Key, g => g.Count())
                .OrderByDescending(kvp => kvp.Value)
                .Take(10)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);

            NetGrowth = commits.Sum(c => c.Additions) - commits.Sum(c => c.Deletions);

            if (commits.Count > 1 && commits[0].Date.HasValue && commits[^1].Date.HasValue)
            {
                var timeSpan = commits[0].Date.Value - commits[^1].Date.Value;
                CommitVelocity = commits.Count / timeSpan.TotalDays;
            }
        }
    }
}
