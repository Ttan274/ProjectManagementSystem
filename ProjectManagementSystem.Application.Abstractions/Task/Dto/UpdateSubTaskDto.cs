namespace ProjectManagementSystem.Application.Abstractions.Dto
{
    public class UpdateSubTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid TaskId { get; set; }
    }
}
