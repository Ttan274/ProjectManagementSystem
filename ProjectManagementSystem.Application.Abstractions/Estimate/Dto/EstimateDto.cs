using ProjectManagementSystem.Application.Abstractions.Base;

namespace ProjectManagementSystem.Application.Abstractions.Estimate.Dto
{
    public class EstimateDto : BaseDto
    {
        public string? Title { get; set; }
        public string? SprintName { get; set; }
        public int Type { get; set; }
        public int TotalEstimate { get; set; } 
        public string TypeDescription { get; set; } = string.Empty;
    }
}
