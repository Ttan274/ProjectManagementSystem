using System.Globalization;

namespace ProjectManagementSystem.Application.Abstractions.Sprint.Dto
{
    public class SprintProgressChartDto
    {
        public SprintProgressChartDto()
        {
            Labels = [];
            IdealBurndown = [];
            ActualBurndown = [];
        }
        public List<string> Labels { get; set; }
        public List<int> IdealBurndown { get; set; }
        public List<int> ActualBurndown { get; set; }

        public class Builder
        {
            private readonly SprintDetailsDto sprint;

            public Builder(SprintDetailsDto sprint)
            {
                ArgumentNullException.ThrowIfNull(sprint);

                this.sprint = sprint;
            }

            public SprintProgressChartDto Build()
            {
                if (!sprint.StartDate.HasValue || !sprint.FinishDate.HasValue)
                    throw new InvalidOperationException("Sprint start and finish dates must be set.");

                var chart = new SprintProgressChartDto();

                var start = sprint.StartDate.Value.Date;
                var finish = sprint.FinishDate.Value.Date;

                for (var date = start; date <= finish; date = date.AddDays(1))
                {
                    chart.Labels.Add(date.ToString("ddd", CultureInfo.InvariantCulture));
                }

                int totalEffort = sprint.Tasks.Sum(t => t.EffortScore ?? 0);

                int sprintLength = chart.Labels.Count;

                for (int day = 0; day < sprintLength; day++)
                {
                    int remaining = totalEffort * (sprintLength - day) / sprintLength;
                    chart.IdealBurndown.Add(remaining);
                }

                for (var date = start; date <= finish; date = date.AddDays(1))
                {
                    int completedEffort = sprint.Tasks
                        .Where(t => t.IsCompleted == true && t.CompletedAt.HasValue && t.CompletedAt.Value.Date <= date)
                        .Sum(t => t.EffortScore ?? 0);

                    chart.ActualBurndown.Add(totalEffort - completedEffort);
                }

                return chart;
            }
        }
    }
}