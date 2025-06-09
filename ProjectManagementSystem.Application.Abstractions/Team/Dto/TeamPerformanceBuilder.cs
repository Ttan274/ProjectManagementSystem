using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;
using ProjectManagementSystem.Common.Consts;

namespace ProjectManagementSystem.Application.Abstractions.Team.Dto
{
    public class TeamPerformanceBuilder(List<SprintDetailsDto> sprintDetailsList, List<UserCommitStatsDto> commitStats)
    {
        private readonly List<SprintDetailsDto> sprintDetailsList = sprintDetailsList;
        private readonly List<UserCommitStatsDto> commitStats = commitStats;

        public TeamPerformanceResult Build()
        {
            var result = new TeamPerformanceResult();

            if (sprintDetailsList == null || !sprintDetailsList.Any())
                return result;

            result.OverallScore = CalculateOverallScore();
            result.AvgTaskCompletion = CalculateAvgTaskCompletion();
            result.AvgOnTimeDelivery = CalculateAvgOnTimeDelivery();
            result.AvgTargetProximity = CalculateAvgTargetProximity();

            result.MemberPerformances = CalculateMemberPerformances();

            return result;
        }

        private double CalculateOverallScore(List<MemberPerformance> allMemberPerformances = null)
        {
            double completionWeight = 0.4;
            double onTimeWeight = 0.3;
            double targetWeight = 0.2;
            double codingWeight = 0.1;

            double completionScore = CalculateAvgTaskCompletion();
            double onTimeScore = CalculateAvgOnTimeDelivery();
            double targetScore = CalculateAvgTargetProximity();
            double codingScore = CalculateAvgCodingScore();

            double rawScore = (completionScore * completionWeight) +
                             (onTimeScore * onTimeWeight) +
                             (targetScore * targetWeight) +
                             (codingScore * codingWeight);

            if (allMemberPerformances != null && allMemberPerformances.Any())
            {
                var allScores = allMemberPerformances.Select(m =>
                    (m.TaskCompletionRate * completionWeight) +
                    (m.OnTimeDeliveryRate * onTimeWeight) +
                    (m.CodingScore * codingWeight))
                    .ToList();

                allScores.Add(rawScore);

                double minScore = allScores.Min();
                double maxScore = allScores.Max();
                double range = maxScore - minScore;

                if (range > 0)
                {
                    return ((rawScore - minScore) / range) * 100;
                }
            }

            return Math.Max(0, Math.Min(100, rawScore));
        }
        private double CalculateAvgTaskCompletion()
        {
            var allTasks = GetAllTasks();
            if (!allTasks.Any())
                return 0;

            var completedTasks = allTasks.Count(t => t.IsCompleted == true);
            return (double)completedTasks / allTasks.Count * 100;
        }

        private double CalculateAvgOnTimeDelivery()
        {
            var allTasksWithSprints = GetAllTasksWithSprints();
            if (!allTasksWithSprints.Any())
                return 0;

            var completedTasks = allTasksWithSprints
                .Where(t => t.Task.IsCompleted == true && t.Task.CompletedAt != null);

            if (!completedTasks.Any())
                return 0;

            var onTimeTasks = completedTasks.Count(t => t.Task.CompletedAt <= t.Sprint.FinishDate);
            var percentage = (double)onTimeTasks / completedTasks.Count() * 100;

            return Math.Round(percentage, 2);
        }

        private double CalculateAvgTargetProximity()
        {
            var allTasks = GetAllTasks();
            if (!allTasks.Any())
                return 0;

            var completedTasks = allTasks.Where(t => t.IsCompleted == true).ToList();
            if (!completedTasks.Any())
                return 0;

            double totalEffort = allTasks.Sum(t => t.EffortScore ?? 0);
            double completedEffort = completedTasks.Sum(t => t.EffortScore ?? 0);

            if (totalEffort == 0)
                return 0;

            return (completedEffort / totalEffort) * 100;
        }

        private double CalculateAvgCodingScore()
        {
            if (commitStats == null || !commitStats.Any())
                return 0;

            return commitStats.Average(c => c.CodingScore);
        }

        private List<MemberPerformance> CalculateMemberPerformances()
        {
            var memberPerformances = new Dictionary<string, MemberPerformance>();
            var allTasksWithSprints = GetAllTasksWithSprints();

            if (!allTasksWithSprints.Any())
                return new List<MemberPerformance>();

            var allMembers = allTasksWithSprints
                .Where(t => !string.IsNullOrEmpty(t.Task.AssignedMember) && t.Task.AssignedMember != Defaults.UNKNOWN_USER)
                .Select(t => t.Task.AssignedMember)
                .Distinct()
                .ToList();

            var tempPerformances = new List<MemberPerformance>();

            foreach (var member in allMembers)
            {
                var memberTasks = allTasksWithSprints
                    .Where(t => t.Task.AssignedMember == member)
                    .ToList();

                var commitStat = commitStats?.FirstOrDefault(c => c.Username == member);

                var performance = new MemberPerformance
                {
                    MemberName = member,
                    TaskCompletionRate = CalculateMemberTaskCompletion(memberTasks),
                    OnTimeDeliveryRate = CalculateMemberOnTimeDelivery(memberTasks),
                    CodingScore = commitStat?.CodingScore ?? 0
                };

                tempPerformances.Add(performance);
            }

            foreach (var performance in tempPerformances)
            {
                var memberTasks = allTasksWithSprints
                    .Where(t => t.Task.AssignedMember == performance.MemberName)
                    .ToList();

                var commitStat = commitStats?.FirstOrDefault(c => c.Username == performance.MemberName);

                performance.OverallScore = CalculateNormalizedOverallScore(memberTasks, commitStat, tempPerformances);

                performance.PerformanceLabel = GetPerformanceLabel(performance.OverallScore);
                memberPerformances[performance.MemberName ?? Defaults.UNKNOWN_USER] = performance;
            }

            return memberPerformances.Values.OrderByDescending(m => m.OverallScore).ToList();
        }
        private double CalculateMemberTaskCompletion(List<(TaskSummaryDto Task, SprintDetailsDto Sprint)> memberTasks)
        {
            if (!memberTasks.Any())
                return 0;

            var completedTasks = memberTasks.Count(t => t.Task.IsCompleted == true);
            return (double)completedTasks / memberTasks.Count * 100;
        }

        private double CalculateMemberOnTimeDelivery(List<(TaskSummaryDto Task, SprintDetailsDto Sprint)> memberTasks)
        {
            if (!memberTasks.Any())
                return 0;

            var sprintGroups = memberTasks
                .Where(t => t.Task.IsCompleted == true && t.Sprint.FinishDate != null)
                .GroupBy(t => t.Sprint.Id);

            if (!sprintGroups.Any())
                return 0;

            double totalOnTimeRate = 0;
            int countedSprints = 0;

            foreach (var group in sprintGroups)
            {
                var sprintFinishDate = group.First().Sprint.FinishDate;
                var completedTasks = group.Select(t => t.Task).ToList();
                var onTimeTasks = completedTasks.Count(t => t.CompletedAt <= sprintFinishDate);
                double sprintOnTimeRate = (double)onTimeTasks / completedTasks.Count * 100;

                totalOnTimeRate += sprintOnTimeRate;
                countedSprints++;
            }

            var average = countedSprints > 0 ? totalOnTimeRate / countedSprints : 0;
            return Math.Round(average, 2);
        }

        public double CalculateNormalizedOverallScore(List<(TaskSummaryDto Task, SprintDetailsDto Sprint)> memberTasks,
                                         UserCommitStatsDto? commitStat,
                                         List<MemberPerformance> allMemberPerformances)
        {
            double completionRate = CalculateMemberTaskCompletion(memberTasks);
            double onTimeRate = CalculateMemberOnTimeDelivery(memberTasks);
            double codingScore = commitStat?.CodingScore ?? 0;

            double overallScore = (completionRate * 0.4) + (onTimeRate * 0.4) + (codingScore * 0.2);

            var allScores = allMemberPerformances.Select(m => m.OverallScore).ToList();

            double minScore = allScores.Min();
            double maxScore = allScores.Max();
            double range = maxScore - minScore;

            double normalizedScore = range > 0 ? ((overallScore - minScore) / range) * 100 : 50;

            return allScores.Count(s => s <= overallScore) / (double)allScores.Count * 100;
        }

        private string GetPerformanceLabel(double overallScore)
        {
            if (overallScore >= 90) return "Leader";
            if (overallScore >= 75) return "Good";
            if (overallScore >= 60) return "Average";
            return "Needs Improvement";
        }

        private List<TaskSummaryDto> GetAllTasks()
        {
            return sprintDetailsList
                .SelectMany(s => s.Tasks ?? Enumerable.Empty<TaskSummaryDto>())
                .ToList();
        }

        private List<(TaskSummaryDto Task, SprintDetailsDto Sprint)> GetAllTasksWithSprints()
        {
            return sprintDetailsList
                .SelectMany(s => s.Tasks?
                    .Select(t => (Task: t, Sprint: s))
                    ?? Enumerable.Empty<(TaskSummaryDto, SprintDetailsDto)>())
                .ToList();
        }
    }

    public class TeamPerformanceResult
    {
        public TeamPerformanceResult()
        {
            MemberPerformances = new List<MemberPerformance>();
        }
        public double OverallScore { get; set; }
        public double AvgTaskCompletion { get; set; }
        public double AvgOnTimeDelivery { get; set; }
        public double AvgTargetProximity { get; set; }
        public List<MemberPerformance> MemberPerformances { get; set; }
    }

    public class MemberPerformance
    {
        public string? MemberName { get; set; }
        public double TaskCompletionRate { get; set; }
        public double OnTimeDeliveryRate { get; set; }
        public double CodingScore { get; set; }
        public string? PerformanceLabel { get; set; }

        public double OverallScore { get; set; }
    }
}
