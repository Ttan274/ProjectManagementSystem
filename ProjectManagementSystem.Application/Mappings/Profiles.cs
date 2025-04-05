using AutoMapper;
using ProjectManagementSystem.Application.Abstractions.Project.Dto;
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
        }
    }
}
