using ProjectManagementSystem.Application.Abstractions.Base;
using ProjectManagementSystem.Application.Abstractions.Project.Dto;
using ProjectManagementSystem.Application.Abstractions.User.Dtos;

namespace ProjectManagementSystem.Application.Abstractions.Team.Dto
{
    public class TeamDto : BaseDto
    {
        public string? TeamName { get; set; }
        public string? TeamDesc { get; set; }
        public ICollection<ProjectDto>? Projects { get; set; }
        public ICollection<UserDto>? Users { get; set; }
        public ProjectDto? ProjectDto { get; set; }
    }
}
