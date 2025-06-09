namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class WorkflowAnalysis : ICommitAnalysis
    {
        public WorkflowAnalysis()
        {
            AnalysisName = "Workflow Patterns";
            CommitsByHour = [];
            CommitsByDay = [];
            BusyPeriods = [];
            IdlePeriods = [];
        }
        public WorkflowAnalysis(string projectName)
        {
            AnalysisName = projectName + " Workflow Patterns";
            CommitsByHour = [];
            CommitsByDay = [];
            BusyPeriods = [];
            IdlePeriods = [];
        }
        public string AnalysisName { get; set; }
        public Dictionary<int, int> CommitsByHour { get; private set; } = new();
        public Dictionary<DayOfWeek, int> CommitsByDay { get; private set; } = new();
        public List<DateTime> BusyPeriods { get; private set; } = new();
        public List<DateTime> IdlePeriods { get; private set; } = new();

        public void Analyze(List<GitCommitDto> commits)
        {
            CommitsByHour = commits
                .Where(c => c.Date.HasValue)
                .GroupBy(c => c.Date.Value.Hour)
                .ToDictionary(g => g.Key, g => g.Count());

            CommitsByDay = commits
                .Where(c => c.Date.HasValue)
                .GroupBy(c => c.Date.Value.DayOfWeek)
                .ToDictionary(g => g.Key, g => g.Count());

            BusyPeriods = commits
                .GroupBy(c => c.Date?.Date)
                .Where(g => g.Count() > 5)
                .Select(g => g.Key.Value)
                .ToList();

            var commitDates = commits
                .Where(c => c.Date.HasValue)
                .Select(c => c.Date.Value.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToList();

            for (int i = 1; i < commitDates.Count; i++)
            {
                var diff = (commitDates[i] - commitDates[i - 1]).Days;
                if (diff > 7)
                {
                    IdlePeriods.Add(commitDates[i - 1].AddDays(1));
                }
            }
        }
    }
}
