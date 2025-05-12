namespace ProjectManagementSystem.Models
{
    public class NavBarItem
    {
        public string? Role { get; set; }
        public string? Title { get; set; }
        public string? Action { get; set; }
        public string? Controller { get; set; }
        public List<string>? VisibleFor { get; set; }
    }
}
