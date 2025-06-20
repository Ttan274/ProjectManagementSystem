using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.AppInfo;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.GithubDependency;
using ProjectManagementSystem.Controllers.Base;

namespace ProjectManagementSystem.Controllers;
public class GitDependencyController(
    IMapper mapper,
    IAppInfoService appInfoService,
    IGithubDependencyService githubDependencyService) : BaseController
{
    [HttpGet]
    public async Task<IActionResult> GetAppDependencies(Guid appId)
    {
        if (appId == Guid.Empty)
        {
            return BadRequest(Error("Invalid request"));
        }

        try
        {
            var appInfoResponse = await appInfoService
                .GetByIdAsync(appId)
                .ConfigureAwait(false);

            var appInfo = appInfoResponse.Data;

            if (appInfo == null || !appInfoResponse.Success)
            {
                return BadRequest(Error(appInfoResponse?.ErrorMessage ?? "Internal server error occured."));
            }

            var appCredentials = mapper.Map<AppGitCredentialDto>(appInfo);

            var appDependencyResponse = await githubDependencyService
                .GetDependencyGraphAsync(appCredentials)
                .ConfigureAwait(false);

            if (!appDependencyResponse.Success)
            {
                return BadRequest(Error(appDependencyResponse?.ErrorMessage ?? "Internal server error occured."));
            }

            return PartialView("_Dependency", appDependencyResponse.Data);
        }
        catch (Exception)
        {
            return BadRequest(Error("Internal server error occured."));
        }
    }
}
