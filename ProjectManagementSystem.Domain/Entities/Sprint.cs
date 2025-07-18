﻿using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class Sprint : BaseEntity
    {
        public Sprint()
        {
            Tasks = new HashSet<Task>();
        }
        public string? SprintName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public bool? Completed { get; set; }
        //Connections
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
        public ICollection<Task>? Tasks { get; set; } = [];
        public ICollection<Estimate>? Estimates { get; set; }
    }
}
