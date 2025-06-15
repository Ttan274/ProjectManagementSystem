using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig.Dto;

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
        public double RefactorScore { get; set; }

        public double CommitEfficiency { get; set; }
        public double CalculateScore(ProjectTeamConfigDto? projectTeamConfig)
        {
            double commitWeight = projectTeamConfig?.CommitWeight ?? 0.1;
            double netChangeWeight = projectTeamConfig?.NetChangeWeight ?? 0.5;
            double refactorWeight = projectTeamConfig?.RefactorWeight ?? 0.4;

            double score = (CommitCount * commitWeight) +
                           (NetChanges * netChangeWeight) +
                           (EstimatedRefactoredLines * refactorWeight);

            return score;
        }

        public void SetCodingScore(ProjectTeamConfigDto? projectTeamConfig) => CodingScore = CalculateScore(projectTeamConfig);

        public void CalculateRefactorScore()
        {
            if (TotalAdditions + TotalDeletions == 0)
            {
                RefactorScore = 0;
                return;
            }

            double refactorRatio = (double)EstimatedRefactoredLines / (TotalAdditions + TotalDeletions);

            // normalize between 0-100
            RefactorScore = Math.Min(100, refactorRatio * 100);
        }

        public void CalculateCommitEfficiency()
        {
            if (CommitCount == 0)
            {
                CommitEfficiency = 0;
                return;
            }

            // changes per commit
            double changesPerCommit = (double)(TotalAdditions + TotalDeletions) / CommitCount;

            // ideal commit size
            double idealCommitSize = 125;
            double efficiency;

            if (changesPerCommit <= idealCommitSize)
            {
                // less commits are valuable
                efficiency = (changesPerCommit / idealCommitSize) * 100;
            }
            else
            {
                // more commits are less valuable
                efficiency = Math.Max(20, 100 - ((changesPerCommit - idealCommitSize) / idealCommitSize * 50));
            }

            CommitEfficiency = Math.Min(100, Math.Max(0, efficiency));
        }

        public void CalculateAllScores(ProjectTeamConfigDto? projectTeamConfig)
        {
            SetCodingScore(projectTeamConfig);
            CalculateRefactorScore();
            CalculateCommitEfficiency();
        }
    }
}
