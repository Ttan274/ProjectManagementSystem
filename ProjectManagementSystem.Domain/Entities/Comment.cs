using ProjectManagementSystem.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Comment : BaseEntity
    {
        public string? Header { get; set; }
        public string? Text { get; set; }

        //Connections
        public Guid DocumentationId { get; set; }
        public Documentation? Documentation { get; set; }
    }
}
