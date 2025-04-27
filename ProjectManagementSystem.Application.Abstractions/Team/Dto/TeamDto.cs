using ProjectManagementSystem.Application.Abstractions.Base;

namespace ProjectManagementSystem.Application.Abstractions.Team.Dto
{
    public class TeamDto : BaseDto
    {
        public string? TeamName { get; set; }
        public string? TeamDesc { get; set; }
        public ICollection<Domain.Entities.Project>? Projects { get; set; }
    }
}
