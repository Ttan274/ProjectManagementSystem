using ProjectManagementSystem.Application.Abstractions.Team.Dto;
using ProjectManagementSystem.Application.Abstractions.User.Dtos;

namespace ProjectManagementSystem.ViewModel
{
    public class AdminViewModel
    {
        public UserDto? UserToCreate { get; set; }
        public TeamDto? TeamToCreate { get; set; }
        public IEnumerable<UserDto>? Users { get; set; }
        public IEnumerable<TeamDto>? Teams { get; set; }
    }
}
