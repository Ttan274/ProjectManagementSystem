using Microsoft.AspNetCore.Identity;
using ProjectManagementSystem.Common.Enums;

namespace ProjectManagementSystem.Domain.Entities
{
    public class AppUser : IdentityUser<Guid>
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public Gender Gender { get; set; }
        public bool Status { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //Connections
        public Guid? TeamId { get; set; }
        public Team? Team { get; set; }
    }
}
