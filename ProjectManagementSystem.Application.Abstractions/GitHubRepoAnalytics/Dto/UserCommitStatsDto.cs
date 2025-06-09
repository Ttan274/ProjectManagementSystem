namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class UserCommitStatsDto
    {
        public UserCommitStatsDto()
        {

        }
        public string? UserId { get; set; }
        public string? Username { get; set; }
        public int CommitCount { get; set; }
        public int TotalAdditions { get; set; }
        public int TotalDeletions { get; set; }
        public int EstimatedRefactoredLines { get; set; }
        public int NetChanges => TotalAdditions - TotalDeletions;
        public double CodingScore { get; private set; }

        public double CalculateScore()
        {
            double commitWeight = 0.1;
            double netChangeWeight = 0.5;
            double refactorWeight = 0.4;

            double score = (CommitCount * commitWeight) +
                           (NetChanges * netChangeWeight) +
                           (EstimatedRefactoredLines * refactorWeight);

            return score;
        }

        public void SetCodingScore() => CodingScore = CalculateScore();
    }
}
