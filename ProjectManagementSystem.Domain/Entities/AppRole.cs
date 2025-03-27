using Microsoft.AspNetCore.Identity;

namespace ProjectManagementSystem.Domain.Entities
{
    public class AppRole : IdentityRole<Guid>
    {
        public string? Description { get; set; }
        public bool Status { get; set; } = true;
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
