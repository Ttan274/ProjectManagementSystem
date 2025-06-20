namespace ProjectManagementSystem.Models
{
    public class NavBarItem
    {
        public string? Name { get; set; }
        public string? Role { get; set; }
        public string? IconClass { get; set; }
        public string? Title { get; set; }
        public string? Action { get; set; }
        public string? Controller { get; set; }
        public List<string>? VisibleFor { get; set; }
        public string? ElementId { get; set; }
    }
}
