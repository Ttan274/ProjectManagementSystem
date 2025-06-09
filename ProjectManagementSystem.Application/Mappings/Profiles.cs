using AutoMapper;
using Octokit;
using ProjectManagementSystem.Application.Abstractions.AppInfo.Dto;
using ProjectManagementSystem.Application.Abstractions.Documentation.Dto;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto;
using ProjectManagementSystem.Application.Abstractions.Project.Dto;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;
using ProjectManagementSystem.Application.Abstractions.Team.Dto;
using ProjectManagementSystem.Application.Abstractions.User.Dtos;

namespace ProjectManagementSystem.Application.Mappings
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<Domain.Entities.AppUser, UserDto>().ReverseMap();
            CreateMap<Domain.Entities.Team, TeamDto>().ReverseMap();
            CreateMap<Domain.Entities.Project, ProjectDto>().ReverseMap();
            CreateMap<Domain.Entities.Sprint, SprintDto>().ReverseMap();
            CreateMap<Domain.Entities.Task, TaskDto>().ReverseMap();
            CreateMap<Domain.Entities.Documentation, DocumentationDto>().ReverseMap();
            CreateMap<Domain.Entities.AppInfo, AppGitCredentialDto>()
                .ForMember(dest => dest.PatToken, opt => opt.MapFrom(src => src.GitHubPatToken))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.GitHubOwner))
                .ForMember(dest => dest.RepoName, opt => opt.MapFrom(src => src.GitHubRepo));
            CreateMap<Domain.Entities.AppInfo, AppInfoDto>().ReverseMap();
            CreateMap<Domain.Entities.AppInfo, CreateAppInfoDto>().ReverseMap();
            CreateMap<AppInfoDto, CreateAppInfoDto>().ReverseMap();
            CreateMap<CreateAppInfoDto, UpdateAppInfoDto>().ReverseMap();
            CreateMap<AppInfoDto, AppGitCredentialDto>()
                .ForMember(dest => dest.PatToken, opt => opt.MapFrom(src => src.GitHubPatToken))
                .ForMember(dest => dest.Owner, opt => opt.MapFrom(src => src.GitHubOwner))
                .ForMember(dest => dest.RepoName, opt => opt.MapFrom(src => src.GitHubRepo));
            CreateMap<GitHubCommitFile, GitCommitFileDto>().ReverseMap();
        }
    }
}
