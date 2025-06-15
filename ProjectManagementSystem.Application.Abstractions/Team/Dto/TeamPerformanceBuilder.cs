using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto;
using ProjectManagementSystem.Application.Abstractions.ProjectTeamConfig.Dto;
using ProjectManagementSystem.Application.Abstractions.Sprint.Dto;
using ProjectManagementSystem.Application.Abstractions.Task.Dto;
using ProjectManagementSystem.Common.Consts;

namespace ProjectManagementSystem.Application.Abstractions.Team.Dto
{
    public class TeamPerformanceBuilder
    {
        private readonly List<SprintDetailsDto> sprintDetailsList;
        private readonly List<UserCommitStatsDto> commitStats;
        private readonly ProjectTeamConfigDto projectTeamConfig;
        private readonly List<(TaskSummaryDto Task, SprintDetailsDto Sprint)> allTasksWithSprints;

        private readonly double[] weights;

        public TeamPerformanceBuilder(List<SprintDetailsDto> sprintDetailsList,
                                      List<UserCommitStatsDto> commitStats,
                                      ProjectTeamConfigDto projectTeamConfig)
        {
            this.sprintDetailsList = sprintDetailsList ?? throw new ArgumentNullException(nameof(sprintDetailsList));
            this.commitStats = commitStats ?? throw new ArgumentNullException(nameof(commitStats));
            this.projectTeamConfig = projectTeamConfig ?? throw new ArgumentNullException(nameof(projectTeamConfig));
            allTasksWithSprints = GetAllTasksWithSprints();

            weights =
            [
                projectTeamConfig.TaskCompletionWeight,
                projectTeamConfig.OnTimeDeliveryWeight,
                projectTeamConfig.TargetProximityWeight,
                projectTeamConfig.CodingScoreWeight,
                projectTeamConfig.RefactorWeight,
                projectTeamConfig.CommitWeight
            ];
        }

        public TeamPerformanceResult Build()
        {
            if (!sprintDetailsList.Any())
                return new TeamPerformanceResult();

            var metrics = CalculateTeamMetrics();
            var memberPerformances = CalculateMemberPerformances();

            return new TeamPerformanceResult
            {
                OverallScore = Math.Round(CalculateWeightedScore(metrics), 2),
                AvgTaskCompletion = Math.Round(metrics.taskCompletion, 2),
                AvgOnTimeDelivery = Math.Round(metrics.onTimeDelivery, 2),
                AvgTargetProximity = Math.Round(metrics.targetProximity, 2),
                AvgCodingScore = Math.Round(metrics.codingScore, 2),
                AvgRefactorRate = Math.Round(metrics.refactorRate, 2),
                AvgCommitEfficiency = Math.Round(metrics.commitEfficiency, 2),
                MemberPerformances = memberPerformances
            };
        }

        private (double taskCompletion, double onTimeDelivery, double targetProximity,
                double codingScore, double refactorRate, double commitEfficiency) CalculateTeamMetrics()
        {
            var allTasks = GetAllTasks();

            return (
                taskCompletion: CalculateTaskCompletionRate(allTasks),
                onTimeDelivery: CalculateOnTimeDeliveryRate(),
                targetProximity: CalculateTargetProximityRate(allTasks),
                codingScore: CalculateAverageMetric(commitStats, c => c.CodingScore),
                refactorRate: CalculateAverageMetric(commitStats, c => c.RefactorScore),
                commitEfficiency: CalculateAverageMetric(commitStats, c => c.CommitEfficiency)
            );
        }

        private double CalculateWeightedScore((double taskCompletion, double onTimeDelivery, double targetProximity,
                                             double codingScore, double refactorRate, double commitEfficiency) metrics)
        {
            var scores = new[] { metrics.taskCompletion, metrics.onTimeDelivery, metrics.targetProximity,
                               metrics.codingScore, metrics.refactorRate, metrics.commitEfficiency };

            return CalculateWeightedAverage(scores, weights);
        }

        private double CalculateWeightedScore(params double[] scores)
        {
            return CalculateWeightedAverage(scores, weights);
        }

        private static double CalculateWeightedAverage(double[] scores, double[] weights)
        {
            if (scores.Length != weights.Length)
                throw new ArgumentException("Scores and weights arrays must have the same length");

            double weightedSum = scores.Zip(weights, (score, weight) => score * weight).Sum();
            double totalWeight = weights.Sum();

            return totalWeight > 0 ? Math.Clamp(weightedSum / totalWeight, 0, 100) : 0;
        }

        private double CalculateTaskCompletionRate(IEnumerable<TaskSummaryDto> tasks)
        {
            var taskList = tasks.ToList();
            return taskList.Any() ? CalculateCompletionRate(taskList) : 0;
        }

        private double CalculateOnTimeDeliveryRate()
        {
            if (!allTasksWithSprints.Any()) return 0;

            var completedTasks = allTasksWithSprints
                .Where(t => t.Task.IsCompleted == true && t.Task.CompletedAt.HasValue)
                .ToList();

            return completedTasks.Any()
                ? CalculateOnTimeRate(completedTasks, t => t.Task.CompletedAt <= t.Sprint.FinishDate)
                : 0;
        }

        private double CalculateTargetProximityRate(IEnumerable<TaskSummaryDto> allTasks)
        {
            var taskList = allTasks.ToList();
            if (!taskList.Any()) return 0;

            var completedTasks = taskList.Where(t => t.IsCompleted == true).ToList();
            if (!completedTasks.Any()) return 0;

            double totalEffort = taskList.Sum(t => t.EffortScore ?? 0);
            double completedEffort = completedTasks.Sum(t => t.EffortScore ?? 0);

            return totalEffort > 0 ? (completedEffort / totalEffort) * 100 : 0;
        }

        private static double CalculateAverageMetric<T>(IEnumerable<T> items, Func<T, double> selector)
        {
            if (items?.Any() != true) return 0;

            return items.Select(selector)
                       .Select(score => Math.Clamp(score, 0, 100))
                       .Average();
        }

        private List<MemberPerformance> CalculateMemberPerformances()
        {
            var memberGroups = allTasksWithSprints
                .Where(t => !string.IsNullOrWhiteSpace(t.Task.AssignedMember) &&
                           t.Task.AssignedMember != Defaults.UNKNOWN_USER)
                .GroupBy(t => t.Task.AssignedMember)
                .ToList();

            var memberPerformances = memberGroups
                .Select(g => CreateMemberPerformance(g.Key, g.ToList()))
                .OrderByDescending(m => m.OverallScore)
                .ToList();

            return memberPerformances;
        }

        private MemberPerformance CreateMemberPerformance(string memberName,
            List<(TaskSummaryDto Task, SprintDetailsDto Sprint)> memberTasks)
        {
            var commitStat = commitStats?.FirstOrDefault(c => c.Username == memberName);
            var memberTaskList = memberTasks.Select(t => t.Task).ToList();

            double completionRate = CalculateTaskCompletionRate(memberTaskList);
            double onTimeRate = CalculateMemberOnTimeDelivery(memberTasks);
            double targetProximity = CalculateTargetProximityRate(memberTaskList);
            double codingScore = Math.Clamp(commitStat?.CodingScore ?? 0, 0, 100);
            double refactorScore = Math.Clamp(commitStat?.RefactorScore ?? 0, 0, 100);
            double commitEfficiency = Math.Clamp(commitStat?.CommitEfficiency ?? 0, 0, 100);

            double overallScore = CalculateWeightedScore(
                completionRate, onTimeRate, targetProximity,
                codingScore, refactorScore, commitEfficiency);

            return new MemberPerformance
            {
                MemberName = memberName,
                TaskCompletionRate = Math.Round(completionRate, 2),
                OnTimeDeliveryRate = Math.Round(onTimeRate, 2),
                TargetProximity = Math.Round(targetProximity, 2),
                CodingScore = Math.Round(codingScore, 2),
                RefactorScore = Math.Round(refactorScore, 2),
                CommitEfficiency = Math.Round(commitEfficiency, 2),
                OverallScore = Math.Round(overallScore, 2),
                PerformanceLabel = GetPerformanceLabel(overallScore)
            };
        }

        private double CalculateMemberOnTimeDelivery(List<(TaskSummaryDto Task, SprintDetailsDto Sprint)> memberTasks)
        {
            if (!memberTasks.Any()) return 0;

            var completedTasksBySprint = memberTasks
                .Where(t => t.Task.IsCompleted == true && t.Sprint.FinishDate.HasValue)
                .GroupBy(t => t.Sprint.Id)
                .ToList();

            if (!completedTasksBySprint.Any()) return 0;

            var sprintOnTimeRates = completedTasksBySprint.Select(group =>
            {
                var sprintFinishDate = group.First().Sprint.FinishDate;
                return CalculateOnTimeRate(group.ToList(), t => t.Task.CompletedAt <= sprintFinishDate);
            });

            return sprintOnTimeRates.Average();
        }

        private static double CalculateCompletionRate(IEnumerable<TaskSummaryDto> tasks)
        {
            var taskList = tasks.ToList();
            int totalTasks = taskList.Count;
            int completedTasks = taskList.Count(t => t.IsCompleted == true);

            return totalTasks > 0 ? (double)completedTasks / totalTasks * 100 : 0;
        }

        private static double CalculateOnTimeRate<T>(IEnumerable<T> items, Func<T, bool> isOnTimePredicate)
        {
            var itemList = items.ToList();
            int totalItems = itemList.Count;
            int onTimeItems = itemList.Count(isOnTimePredicate);

            return totalItems > 0 ? (double)onTimeItems / totalItems * 100 : 0;
        }

        private static string GetPerformanceLabel(double overallScore) => overallScore switch
        {
            >= 90 => "Excellent",
            >= 80 => "Very Good",
            >= 70 => "Good",
            >= 60 => "Satisfactory",
            >= 50 => "Needs Improvement",
            _ => "Poor"
        };

        private List<TaskSummaryDto> GetAllTasks()
        {
            return sprintDetailsList
                .SelectMany(s => s.Tasks ?? Enumerable.Empty<TaskSummaryDto>())
                .ToList();
        }

        private List<(TaskSummaryDto Task, SprintDetailsDto Sprint)> GetAllTasksWithSprints()
        {
            return sprintDetailsList
                .SelectMany(s => s.Tasks?.Select(t => (Task: t, Sprint: s)) ??
                               Enumerable.Empty<(TaskSummaryDto, SprintDetailsDto)>())
                .ToList();
        }
    }

    public class TeamPerformanceResult
    {
        public double OverallScore { get; set; }
        public double AvgTaskCompletion { get; set; }
        public double AvgOnTimeDelivery { get; set; }
        public double AvgTargetProximity { get; set; }
        public double AvgCodingScore { get; set; }
        public double AvgRefactorRate { get; set; }
        public double AvgCommitEfficiency { get; set; }
        public List<MemberPerformance> MemberPerformances { get; set; } = new();
    }

    public class MemberPerformance
    {
        public string? MemberName { get; set; }
        public double TaskCompletionRate { get; set; }
        public double OnTimeDeliveryRate { get; set; }
        public double TargetProximity { get; set; }
        public double CodingScore { get; set; }
        public double RefactorScore { get; set; }
        public double CommitEfficiency { get; set; }
        public string? PerformanceLabel { get; set; }
        public double OverallScore { get; set; }
    }
}