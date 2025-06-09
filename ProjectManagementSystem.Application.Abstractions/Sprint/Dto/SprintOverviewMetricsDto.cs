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
        public int CompletionRate { get; set; }
        public int Velocity { get; set; }
        public int OnTimeDeliveryRate { get; set; }
        public int TotalTaskCount { get; set; }
        public int TotalStoryPoints { get; set; }
        public double AverageSprintVelocity { get; set; }
        public int PreviousSprintOnTimeDeliveryRate { get; set; }
    }
}
