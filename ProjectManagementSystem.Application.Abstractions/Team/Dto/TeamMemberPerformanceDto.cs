namespace ProjectManagementSystem.Application.Abstractions.Team.Dto
{
    public class TeamMemberPerformanceDto
    {
        public TeamMemberPerformanceDto()
        {

        }

        public Guid Id { get; set; }
        public string? Name { get; set; }
        public string? Role { get; set; }
        public int? TaskCompletionRate { get; set; }
        public int? OnTimeDeliveryRate { get; set; }
        public string? StatusBadge { get; set; }
        public int? StoryPointsCompleted { get; set; }
        public int? BugsResolved { get; set; }
    }
}
