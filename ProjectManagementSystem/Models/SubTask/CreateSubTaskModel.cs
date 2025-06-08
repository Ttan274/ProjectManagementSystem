namespace ProjectManagementSystem.Models
{
    public class CreateSubTaskModel
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public Guid ParentTaskId { get; set; }
    }
}
