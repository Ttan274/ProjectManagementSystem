namespace ProjectManagementSystem.Application.Abstractions.Sprint.Dto
{
    public class SprintCompletionChartDto
    {
        public SprintCompletionChartDto()
        {

        }
        public string SprintName { get; set; } = default!;
        public int PlannedTaskCount { get; set; }
        public int CompletedTaskCount { get; set; }
    }
}
