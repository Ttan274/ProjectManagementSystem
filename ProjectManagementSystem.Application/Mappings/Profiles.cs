using AutoMapper;
using ProjectManagementSystem.Application.Abstractions.User.Dtos;

namespace ProjectManagementSystem.Application.Mappings
{
    public class Profiles : Profile
    {
        public Profiles()
        {
            CreateMap<Domain.Entities.AppUser, UserDto>().ReverseMap();
        }
    }
}
