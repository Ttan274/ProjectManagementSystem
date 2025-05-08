using ProjectManagementSystem.Application.Abstractions.Base;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Team.Dto;
using ProjectManagementSystem.Common.Enums;

namespace ProjectManagementSystem.Application.Abstractions.Project.Dto
{
    public class ProjectDto : BaseDto
    {
        public string? ProjectName { get; set; }
        public string? ProjectDesc { get; set; }
        public int? EstimatedHours { get; set; }
        public Priority? Priority { get; set; }
        public Guid? TeamId { get; set; }
        public TeamDto? Team { get; set; }
        public ICollection<SprintDto>? Sprints { get; set; }
    }
}
