namespace ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig.Dto
{
    public class ProjectTeamConfigDto
    {
        public ProjectTeamConfigDto()
        {

        }
        public Guid Id { get; set; }
        public Guid ProjectId { get; set; }
        public string? TeamIntroduction { get; set; }
        public double TaskCompletionWeight { get; set; }
        public double OnTimeDeliveryWeight { get; set; }
        public double TargetProximityWeight { get; set; }
        public double CodingScoreWeight { get; set; }

        // kodlama parametreleri
        public double CommitWeight { get; set; }
        public double NetChangeWeight { get; set; }
        public double RefactorWeight { get; set; }

        public int TaskCompletionWeightPercentage => (int)(TaskCompletionWeight * 100);
        public int OnTimeDeliveryWeightPercentage => (int)(OnTimeDeliveryWeight * 100);
        public int TargetProximityWeightPercentage => (int)(TargetProximityWeight * 100);
        public int CodingScoreWeightPercentage => (int)(CodingScoreWeight * 100);
        public int CommitWeightPercentage => (int)(CommitWeight * 100);
        public int NetChangeWeightPercentage => (int)(NetChangeWeight * 100);
        public int RefactorWeightPercentage => (int)(RefactorWeight * 100);

        public void Validate(out string validationMessage)
        {
            List<string> errors = [];

            if (ProjectId == Guid.Empty)
            {
                errors.Add("Project is not found.");
            }

            double mainWeightSum = TaskCompletionWeight + OnTimeDeliveryWeight + TargetProximityWeight + CodingScoreWeight;
            if (Math.Abs(mainWeightSum - 1.0) > 0.0001)
            {
                errors.Add("The sum of the delivery performance, target alignment, and overall contribution weights must be equal to 1.");
            }

            double codingWeightSum = CommitWeight + NetChangeWeight + RefactorWeight;
            if (Math.Abs(codingWeightSum - 1.0) > 0.0001)
            {
                errors.Add("The sum of the code activity weights must be equal to 1.");
            }

            validationMessage = string.Join(" ", errors);
        }
    }
}
