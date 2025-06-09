namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class TeamPerformanceAnalysis : ICommitAnalysis
    {
        public TeamPerformanceAnalysis()
        {
            CommitsByCommitter = [];
            AdditionsByCommitter = [];
            DeletionsByCommitter = [];
            AvgCommitSizeByCommitter = [];
            ActiveContributors = [];
        }
        public string AnalysisName => "Team Performance";
        public Dictionary<string, int> CommitsByCommitter { get; private set; } = new();
        public Dictionary<string, int> AdditionsByCommitter { get; private set; } = new();
        public Dictionary<string, int> DeletionsByCommitter { get; private set; } = new();
        public Dictionary<string, double> AvgCommitSizeByCommitter { get; private set; } = new();
        public List<string> ActiveContributors { get; private set; } = new();

        public void Analyze(List<GitCommitDto> commits)
        {
            CommitsByCommitter = commits
                .GroupBy(c => c.Committer)
                .ToDictionary(g => g.Key ?? "Unknown", g => g.Count());

            AdditionsByCommitter = commits
                .GroupBy(c => c.Committer)
                .ToDictionary(g => g.Key ?? "Unknown", g => g.Sum(c => c.Additions));

            DeletionsByCommitter = commits
                .GroupBy(c => c.Committer)
                .ToDictionary(g => g.Key ?? "Unknown", g => g.Sum(c => c.Deletions));

            AvgCommitSizeByCommitter = commits
                .GroupBy(c => c.Committer)
                .ToDictionary(g => g.Key ?? "Unknown",
                             g => g.Average(c => c.Additions + c.Deletions));

            ActiveContributors = CommitsByCommitter
                .OrderByDescending(kvp => kvp.Value)
                .Take(5)
                .Select(kvp => kvp.Key)
                .ToList();
        }
    }

}
