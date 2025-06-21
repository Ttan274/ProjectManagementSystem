using Microsoft.AspNetCore.Mvc.Rendering;

namespace ProjectManagementSystem.Application.Abstractions.Estimate.Dto
{
    public class CreateEstimateDto
    {
        public string? Title { get; set; }
        public int Type { get; set; }
        public string SprintId { get; set; } = string.Empty;
        public List<SelectListItem> SprintList { get; set; } = [];
        public Guid ProjectId { get; set; }
    }
}
