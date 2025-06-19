using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.GithubDependency;
using ProjectManagementSystem.Application.Abstractions.GithubDependency.Dto;
using ProjectManagementSystem.Common.ServiceResponse;
using System.Net.Http.Headers;
using System.Text.Json;

namespace ProjectManagementSystem.Application.GithubDependency;

public class GithubDependencyService(
    IHttpClientFactory httpClientFactory,
    IServiceResponseHelper serviceResponseHelper) : IGithubDependencyService
{
    public async Task<ServiceResponse<List<DependencyInfoDto>>> GetDependencyGraphAsync(AppGitCredentialDto appGitCredential)
    {
        if (appGitCredential == null)
            return serviceResponseHelper.SetError<List<DependencyInfoDto>>("Git credentials not provided");

        try
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", appGitCredential.PatToken);
            httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("DependencyGraphService", "1.0"));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.github+json"));

            var response = await httpClient.GetAsync(
                $"https://api.github.com/repos/{appGitCredential.Owner}/{appGitCredential.RepoName}/dependency-graph/sbom"
            );

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                return serviceResponseHelper.SetError<List<DependencyInfoDto>>(
                    $"GitHub API error: {response.StatusCode} - {errorContent}"
                );
            }

            var json = await response.Content.ReadAsStringAsync();
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var sbom = JsonSerializer.Deserialize<GitHubSbomResponse>(json, options);

            var dependencies = new List<DependencyInfoDto>();

            foreach (var package in sbom?.Sbom?.Packages ?? Enumerable.Empty<SbomPackage>())
            {
                var purlRef = package?.ExternalRefs?.FirstOrDefault(r => r?.ReferenceType == "purl");
                if (purlRef == null) continue;

                dependencies.Add(new DependencyInfoDto
                {
                    Name = package.Name,
                    Version = package.Version,
                    PackageUrl = purlRef.ReferenceLocator,
                    PackageManager = ParsePackageManager(purlRef.ReferenceLocator),
                    License = package.LicenseConcluded ?? "Unknown"
                });
            }

            return serviceResponseHelper.SetSuccess(dependencies);
        }
        catch (Exception ex)
        {
            return serviceResponseHelper.SetError<List<DependencyInfoDto>>(
                $"Unexpected error: {ex.Message}"
            );
        }
    }

    private static string ParsePackageManager(string purl)
    {
        if (string.IsNullOrEmpty(purl)) return "unknown";

        // pkg:npm/package@version → npm
        // pkg:nuget/package@version → nuget
        var parts = purl.Split(':');
        if (parts.Length < 2) return "unknown";

        var managerPart = parts[1].Split('/').FirstOrDefault();
        return managerPart ?? "unknown";
    }
}