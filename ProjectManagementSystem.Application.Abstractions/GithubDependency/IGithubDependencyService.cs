using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.GithubDependency.Dto;
using ProjectManagementSystem.Common.ServiceResponse;

namespace ProjectManagementSystem.Application.Abstractions.GithubDependency;
public interface IGithubDependencyService
{
    Task<ServiceResponse<List<DependencyInfoDto>>> GetDependencyGraphAsync(AppGitCredentialDto appGitCredential);
}