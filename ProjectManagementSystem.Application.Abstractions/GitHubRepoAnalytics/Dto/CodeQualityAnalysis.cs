namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class CodeQualityAnalysis : ICommitAnalysis
    {
        public CodeQualityAnalysis()
        {
            LargeCommits = [];
        }
        public string AnalysisName => "Code Quality";
        public List<GitCommitDto> LargeCommits { get; private set; }
        public double AvgMessageLength { get; private set; }
        public double AdditionsDeletionsRatio { get; private set; }
        public int RefactoringCommits { get; private set; }
        public int FeatureCommits { get; private set; }

        public void Analyze(List<GitCommitDto> commits)
        {
            const int LargeCommitThreshold = 500;
            LargeCommits = commits
                .Where(c => (c.Additions + c.Deletions) > LargeCommitThreshold)
                .ToList();

            AvgMessageLength = commits
                .Average(c => c.Message?.Length ?? 0);

            var totalAdditions = commits.Sum(c => c.Additions);
            var totalDeletions = commits.Sum(c => c.Deletions);
            AdditionsDeletionsRatio = totalDeletions == 0 ?
                double.MaxValue : totalAdditions / (double)totalDeletions;

            RefactoringCommits = commits.Count(c =>
                c.GetFilesChangedInfos().Any(CommitClassifierHelper.IsRefactor));

            FeatureCommits = commits.Count(c =>
                c.GetFilesChangedInfos().Any(CommitClassifierHelper.IsFeature));
        }
    }
}
