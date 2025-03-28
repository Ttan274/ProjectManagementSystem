using ProjectManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Documentation : BaseEntity
    {
        public string? Header { get; set; }
        public string? Text { get; set; }
        public int? View { get; set; }

        //Connections
        public Guid UserId { get; set; }
        public AppUser? User { get; set; }
        public Guid TaskId { get; set; }
        public Task? Task { get; set; }
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
        public ICollection<Comment>? Comments { get; set; }
    }
}
