using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Domain.Entities
{
    public class ProjectTeamConfig : BaseEntity
    {
        public ProjectTeamConfig()
        {

        }
        public Guid ProjectId { get; set; }
        public Project? Project { get; set; }
        public string? TeamIntroduction { get; set; }
        public double TaskCompletionWeight { get; set; }
        public double OnTimeDeliveryWeight { get; set; }
        public double TargetProximityWeight { get; set; }
        public double CodingScoreWeight { get; set; }

        // kodlama parametreleri
        public double CommitWeight { get; set; }
        public double NetChangeWeight { get; set; }
        public double RefactorWeight { get; set; }
    }
}
