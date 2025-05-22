using ProjectManagementSystem.Application.Abstractions.Base;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Documentation.Dto
{
    public class DocumentationDto : BaseDto
    {
        public string? Header { get; set; }
        public string? Text { get; set; }
        public Guid TaskId { get; set; }
        public TaskDto? Task { get; set; }
    }
}
