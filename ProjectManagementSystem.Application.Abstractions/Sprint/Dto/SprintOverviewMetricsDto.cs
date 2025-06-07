namespace ProjectManagementSystem.Application.Abstractions.Sprint.Dto
{
    public class SprintOverviewMetricsDto
    {
        public SprintOverviewMetricsDto()
        {

        }

        public int CompletedStoryPoints { get; set; }
        public int RemainingDays { get; set; }
        public int OpenBugCount { get; set; }
        public double AvgCycleTime { get; set; }
        public int DocumentationStatus { get; set; }
    }
}
