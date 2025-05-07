using ProjectManagementSystem.Application.Abstractions.Base;

namespace ProjectManagementSystem.Application.Abstractions.Sprint.Dto
{
    public class SprintDto : BaseDto
    {
        public string? SprintName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? FinishDate { get; set; }
        public Guid? ProjectId { get; set; }
        
        //public ICollection<TaskDto>? Tasks { get; set; }
    }
}
