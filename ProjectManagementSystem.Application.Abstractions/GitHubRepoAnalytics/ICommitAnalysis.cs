using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto;

namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics
{
    public interface ICommitAnalysis
    {
        void Analyze(List<GitCommitDto> commits);
        string AnalysisName { get; }
    }
}
