using Microsoft.AspNetCore.Mvc.Rendering;
using ProjectManagementSystem.Common.Enums;
using ProjectManagementSystem.Domain.Entities;

namespace ProjectManagementSystem.Application.Abstractions.Task.Dto
{
    public class TaskDto
    {
        public string? TaskName { get; set; }
        public string? TaskDesc { get; set; }
        public Priority? Priority { get; set; }
        public ProjecStatus? TaskEffort { get; set; }
        public string? SprintId { get; set; }
        public List<SelectListItem>? SprintList { get; set; }
        public string? UserId { get; set; }
        public List<SelectListItem>? UserList { get; set; }
        public AppUser? AppUser { get; set; }
    }
}
