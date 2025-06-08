using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementSystem.Common.Enums;
using ProjectManagementSystem.Domain.Entities;

namespace ProjectManagementSystem.Application.Abstractions.Task.Dto
{
    public class TaskDto
    {
        public TaskDto()
        {
            SprintList = [];
            UserList = [];
        }
        public Guid Id { get; set; }
        public string? TaskName { get; set; }
        public string? TaskDesc { get; set; }
        public Priority? Priority { get; set; }
        public ProjectStatus? State { get; set; }
        public TaskType Type { get; set; }
        public int EffortScore { get; set; }
        public bool? IsUrgent { get; set; }
        public bool? Completed { get; set; }
        public DateTime? CompletedAt { get; set; }
        public string? SprintId { get; set; }
        public List<SelectListItem>? SprintList { get; set; }
        public string? UserId { get; set; }
        public List<SelectListItem>? UserList { get; set; }
        public AppUser? AppUser { get; set; }
        public Domain.Entities.Documentation? Documentation { get; set; }
        public Guid? TaskId { get; set; }
        public ICollection<TaskDto>? DependentTasks { get; set; }
    }
}
