﻿using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig.Dto;

namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class CommitStatAnalysis : ICommitAnalysis
    {
        private readonly ProjectTeamConfigDto? projectTeamConfig;
        public CommitStatAnalysis(ProjectTeamConfigDto? projectTeamConfig)
        {
            this.projectTeamConfig = projectTeamConfig;
            AnalysisName = "CommitStats";
            CommitStats = [];
        }
        public CommitStatAnalysis()
        {

        }

        public List<UserCommitStatsDto> CommitStats { get; set; }

        public string AnalysisName { get; set; }

        public void Analyze(List<GitCommitDto> commits)
        {
            var userStatsDict = new Dictionary<string, UserCommitStatsDto>();

            foreach (var result in commits)
            {
                var commitDetails = result.GetFilesChangedInfos();
                var username = result.Committer ?? "Serhat";

                if (!userStatsDict.TryGetValue(username, out var value))
                {
                    value = new UserCommitStatsDto
                    {
                        UserId = username,
                        Username = username
                    };
                    userStatsDict[username] = value;
                }

                value.CommitCount++;
                value.TotalAdditions += result.Additions;
                value.TotalDeletions += result.Deletions;

                foreach (var file in commitDetails)
                {
                    var estimatedRefactor = Math.Min(file.Additions, file.Deletions);
                    value.EstimatedRefactoredLines += estimatedRefactor;
                }

                value.CalculateAllScores(projectTeamConfig);
            }

            CommitStats = [.. userStatsDict.Values];
        }
    }
}
