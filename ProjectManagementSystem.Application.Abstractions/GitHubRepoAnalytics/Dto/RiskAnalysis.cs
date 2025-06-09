namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class RiskAnalysis : ICommitAnalysis
    {
        public RiskAnalysis()
        {
            AnalysisName = "Risk Indicators";
            SingleAuthorFiles = [];
            HighRiskCommits = [];
            DeadlineRushes = [];
        }
        public RiskAnalysis(string projectName)
        {
            AnalysisName = projectName + " Risk Indicators";
            SingleAuthorFiles = [];
            HighRiskCommits = [];
            DeadlineRushes = [];
        }
        public string AnalysisName { get; set; }
        public Dictionary<string, string> SingleAuthorFiles { get; private set; }
        public List<GitCommitDto> HighRiskCommits { get; private set; }
        public List<GitCommitDto> DeadlineRushes { get; private set; }

        public void Analyze(List<GitCommitDto> commits)
        {
            var fileAuthors = commits
                .SelectMany(c => c.FilesChanged.Select(f => new { File = f, Author = c.Committer }))
                .GroupBy(x => x.File)
                .Where(g => g.Select(x => x.Author).Distinct().Count() == 1)
                .ToDictionary(g => g.Key, g => g.First().Author);

            SingleAuthorFiles = fileAuthors;

            HighRiskCommits = [.. commits.Where(c => (c.Additions + c.Deletions) > 300 && c.FilesChanged.Count < 3)];

            DeadlineRushes = [.. commits.Where(c => c.Date?.DayOfWeek == DayOfWeek.Friday)];
        }
    }
}
