using ProjectManagementSystem.Application.Abstractions.Project.Dto;

namespace ProjectManagementSystem.Models
{
    public class Navbar
    {
        public Navbar()
        {
            Projects = [];
            NavbarItems = [];
        }
        public string? MainTitle { get; set; }
        public List<ProjectDto> Projects { get; set; }
        public List<NavBarItem> NavbarItems { get; set; } = null!;
    }
}
