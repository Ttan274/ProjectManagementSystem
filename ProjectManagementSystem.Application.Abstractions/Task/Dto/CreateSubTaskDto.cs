namespace ProjectManagementSystem.Application.Abstractions.Dto
{
    public class CreateSubTaskDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ParentTaskId { get; set; }
    }
}
