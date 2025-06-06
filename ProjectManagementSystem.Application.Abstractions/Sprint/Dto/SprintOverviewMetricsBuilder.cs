using ProjectManagementSystem.Common.Enums;

namespace ProjectManagementSystem.Application.Abstractions.Sprint.Dto
{
    public class SprintOverviewMetricsBuilder(SprintDetailsDto sprintDetails)
    {
        private readonly SprintOverviewMetricsDto sprintOverviewMetrics = new();
        public SprintOverviewMetricsBuilder WithCompletedStoryPoints()
        {
            sprintOverviewMetrics.CompletedStoryPoints = sprintDetails
                .Tasks
                .Where(task => task.IsCompleted == true)
                .Select(x => x.EffortScore ?? 0)
                .Sum();

            return this;
        }

        public SprintOverviewMetricsBuilder WithRemainingDays()
        {
            var today = DateTime.Today;
            var deadLine = sprintDetails.FinishDate;

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
            sprintOverviewMetrics.OpenBugCount = sprintDetails
                .Tasks
                .Count(task => task.IsCompleted != true && task.Type == TaskType.BugFix);

            return this;
        }

        public SprintOverviewMetricsBuilder WithAvgCycleTime()
        {
            var completedTasksWithDates = sprintDetails.Tasks
                .Where(t => t.IsCompleted == true && t.CompletedAt.HasValue && t.CreatedAt.HasValue)
                .ToList();

            if (completedTasksWithDates.Count == 0)
            {
                sprintOverviewMetrics.AvgCycleTime = 0;
                return this;
            }

            sprintOverviewMetrics.AvgCycleTime = completedTasksWithDates
                .Average(t =>
                {
                    var cycleTime = t.CompletedAt!.Value - t.CreatedAt!.Value;
                    return cycleTime.TotalDays;
                });

            return this;
        }

        public SprintOverviewMetricsBuilder WithDocumentationStatus()
        {
            var totalTasks = sprintDetails.Tasks.Count;
            if (totalTasks == 0)
            {
                sprintOverviewMetrics.DocumentationStatus = 0;
                return this;
            }

            var documentedTasksCount = sprintDetails.Tasks.Count(t => t.HasDocumentation);

            sprintOverviewMetrics.DocumentationStatus = (int)((double)documentedTasksCount / totalTasks * 100);

            return this;
        }

        public SprintOverviewMetricsDto Build()
        {
            return sprintOverviewMetrics;
        }
    }
}
