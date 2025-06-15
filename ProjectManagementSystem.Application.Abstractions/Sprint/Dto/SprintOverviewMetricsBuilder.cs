using ProjectManagementSystem.Common.Enums;

namespace ProjectManagementSystem.Application.Abstractions.Sprint.Dto
{
    public class SprintOverviewMetricsBuilder(List<SprintDetailsDto> sprintDetails, SprintDetailsDto selectedSprint)
    {
        private readonly SprintOverviewMetricsDto sprintOverviewMetrics = new();
        public SprintOverviewMetricsBuilder WithCompletedStoryPoints()
        {
            sprintOverviewMetrics.CompletedStoryPoints = selectedSprint
                .Tasks
                .Where(task => task.IsCompleted == true)
                .Select(x => x.EffortScore ?? 0)
                .Sum();

            return this;
        }

        public SprintOverviewMetricsBuilder WithRemainingDays()
        {
            var today = DateTime.Today;
            var deadLine = selectedSprint.FinishDate;

            int remainingDays = 0;

            if (deadLine.HasValue)
            {
                remainingDays = (deadLine.Value - today).Days;
            }

            sprintOverviewMetrics.RemainingDays = remainingDays;

            return this;
        }

        public SprintOverviewMetricsBuilder WithOpenBugCount()
        {
            sprintOverviewMetrics.OpenBugCount = sprintDetails.SelectMany(x => x.Tasks)
                .Count(task => task.IsCompleted != true && task.Type == TaskType.BugFix);

            return this;
        }

        public SprintOverviewMetricsBuilder WithAvgCycleTime()
        {
            var completedTasksWithDates = sprintDetails.SelectMany(x => x.Tasks)
                .Where(t => t.IsCompleted == true && t.CompletedAt.HasValue && t.CreatedAt.HasValue)
                .ToList();

            if (completedTasksWithDates.Count == 0)
            {
                sprintOverviewMetrics.AvgCycleTime = 0;
                return this;
            }

            sprintOverviewMetrics.AvgCycleTime = Math.Round(
                completedTasksWithDates.Average(t =>
                {
                    var cycleTime = t.CompletedAt!.Value - t.CreatedAt!.Value;
                    return cycleTime.TotalDays;
                }),
                2
            );

            return this;
        }

        public SprintOverviewMetricsBuilder WithDocumentationStatus()
        {
            var allTasks = sprintDetails.SelectMany(x => x.Tasks).ToList();

            var totalTasks = allTasks.Count;
            if (totalTasks == 0)
            {
                sprintOverviewMetrics.DocumentationStatus = 0;
                return this;
            }

            var documentedTasksCount = allTasks.Count(t => t.HasDocumentation);

            sprintOverviewMetrics.DocumentationStatus = (int)((double)documentedTasksCount / totalTasks * 100);

            return this;
        }

        public SprintOverviewMetricsBuilder WithCompletionRate()
        {
            var totalTasks = selectedSprint.Tasks.Count;
            var completedTasks = selectedSprint.Tasks.Count(t => t.IsCompleted == true);

            sprintOverviewMetrics.CompletionRate = totalTasks == 0
                ? 0
                : (int)((double)completedTasks / totalTasks * 100);

            return this;
        }

        public SprintOverviewMetricsBuilder WithVelocity()
        {
            sprintOverviewMetrics.Velocity = selectedSprint
                .Tasks
                .Where(task => task.IsCompleted == true)
                .Select(task => task.EffortScore ?? 0)
                .Sum();

            return this;
        }

        public SprintOverviewMetricsBuilder WithAverageSprintVelocity()
        {
            var allTasks = sprintDetails.SelectMany(x => x.Tasks).ToList();

            var totalCompletedTasks = allTasks.Count(task => task.IsCompleted == true);
            if (totalCompletedTasks == 0)
            {
                sprintOverviewMetrics.AverageSprintVelocity = 0;
            }
            else
            {
                sprintOverviewMetrics.AverageSprintVelocity = allTasks
                    .Where(task => task.IsCompleted == true)
                    .Select(task => task.EffortScore ?? 0)
                    .Average();
            }

            return this;
        }

        public SprintOverviewMetricsBuilder WithTotalStoryPoints()
        {
            sprintOverviewMetrics.TotalStoryPoints = selectedSprint
                .Tasks
                .Select(task => task.EffortScore ?? 0)
                .Sum();

            return this;
        }

        public SprintOverviewMetricsBuilder WithOnTimeDeliveryRate()
        {
            var completedTasks = sprintDetails.SelectMany(x => x.Tasks)
                .Where(t => t.IsCompleted == true && t.CompletedAt.HasValue && t.CompletedAt.HasValue)
                .ToList();

            if (completedTasks.Count == 0)
            {
                sprintOverviewMetrics.OnTimeDeliveryRate = 0;
                return this;
            }

            var onTimeTasks = completedTasks.Count(t => t.CompletedAt <= selectedSprint.FinishDate);

            sprintOverviewMetrics.OnTimeDeliveryRate = (int)((double)onTimeTasks / completedTasks.Count * 100);

            return this;
        }

        public SprintOverviewMetricsBuilder WithTotalTaskCount()
        {
            sprintOverviewMetrics.TotalTaskCount = selectedSprint.Tasks.Count;
            return this;
        }

        public SprintOverviewMetricsBuilder WithSprintCompletionChart()
        {
            sprintOverviewMetrics.SprintCompletions = sprintDetails
                .OrderBy(x => x.StartDate)
                .Select(sprint => new SprintCompletionChartDto
                {
                    SprintName = sprint.Name ?? "Unknown",
                    PlannedTaskCount = sprint.Tasks.Count,
                    CompletedTaskCount = sprint.Tasks.Count(t => t.IsCompleted == true)
                }).ToList();

            return this;
        }

        public SprintOverviewMetricsDto Build()
        {
            return sprintOverviewMetrics;
        }
    }
}
