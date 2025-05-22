using ProjectManagementSystem.Application.Abstractions.Base;

namespace ProjectManagementSystem.Application.Abstractions.Documentation.Dto
{
    public class DocumentationDto : BaseDto
    {
        public string? Header { get; set; }
        public string? Text { get; set; }
    }
}
