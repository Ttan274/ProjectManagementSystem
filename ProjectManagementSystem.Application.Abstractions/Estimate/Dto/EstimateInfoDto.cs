namespace ProjectManagementSystem.Application.Abstractions.Estimate.Dto
{
    public class EstimateInfoDto
    {
        public Guid ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public ICollection<EstimateDto>? Estimates { get; set; }
    }
}
