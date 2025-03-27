using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Team : BaseEntity
    {
        public string? TeamName { get; set; }
        public string? TeamDesc { get; set; }

        //Connections
        public ICollection<AppUser>? Users { get; set; }
        public ICollection<Project>? Projects { get; set; }
    }
}
