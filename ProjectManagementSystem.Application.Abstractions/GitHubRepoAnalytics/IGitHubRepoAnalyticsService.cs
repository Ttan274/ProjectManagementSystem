using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics
{
    public interface IGitHubRepoAnalyticsService
    {
        Task<ServiceResponse<List<UserCommitStatsDto>>> GetCommitCountsByUserAsync(AppGitCredentialDto appGitCredential);
        Task<ServiceResponse<List<UserPullRequestStatsDto>>> GetPullRequestCountsByUserAsync(AppGitCredentialDto appGitCredential);
        Task<ServiceResponse<List<UserPullRequestTimeStatsDto>>> GetPullRequestTimesAsync(AppGitCredentialDto appGitCredential);
    }
}
