namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class CommitAnalysisResult
    {
        public CommitAnalysisResult()
        {
            BasicMetrics = new();
            TeamPerformance = new();
            CodeQuality = new();
            ProjectHealth = new();
            Workflow = new();
            RiskIndicators = new();
            CommitStatAnalysis = new();
        }
        public CommitSizeAnalysis BasicMetrics { get; set; }
        public TeamPerformanceAnalysis TeamPerformance { get; set; }
        public CodeQualityAnalysis CodeQuality { get; set; }
        public ProjectHealthAnalysis ProjectHealth { get; set; }
        public WorkflowAnalysis Workflow { get; set; }
        public RiskAnalysis RiskIndicators { get; set; }
        public CommitStatAnalysis CommitStatAnalysis { get; set; }

        public Dictionary<string, object> GetAllResults()
        {
            return new Dictionary<string, object>
            {
                [BasicMetrics.AnalysisName] = BasicMetrics,
                [TeamPerformance.AnalysisName] = TeamPerformance,
                [CodeQuality.AnalysisName] = CodeQuality,
                [ProjectHealth.AnalysisName] = ProjectHealth,
                [Workflow.AnalysisName] = Workflow,
                [RiskIndicators.AnalysisName] = RiskIndicators,
                [CommitStatAnalysis.AnalysisName] = CommitStatAnalysis
            };
        }
    }
}
