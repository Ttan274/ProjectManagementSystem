namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class CommitAnalysisBuilder
    {
        private readonly List<ICommitAnalysis> analyses;
        private readonly List<GitCommitDto> commits;

        public CommitAnalysisBuilder(List<GitCommitDto> commits)
        {
            this.commits = commits ?? throw new ArgumentNullException(nameof(commits));
            analyses = [];
        }

        public CommitAnalysisBuilder AddBasicMetrics() => AddAnalysis(new CommitSizeAnalysis());
        public CommitAnalysisBuilder AddTeamPerformance() => AddAnalysis(new TeamPerformanceAnalysis());
        public CommitAnalysisBuilder AddCodeQuality() => AddAnalysis(new CodeQualityAnalysis());
        public CommitAnalysisBuilder AddProjectHealth() => AddAnalysis(new ProjectHealthAnalysis());
        public CommitAnalysisBuilder AddWorkflowAnalysis() => AddAnalysis(new WorkflowAnalysis());
        public CommitAnalysisBuilder AddRiskAnalysis() => AddAnalysis(new RiskAnalysis());
        public CommitAnalysisBuilder AddCommitStatAnalysis() => AddAnalysis(new CommitStatAnalysis());

        public CommitAnalysisBuilder AddAllAnalyses()
        {
            return this
                .AddBasicMetrics()
                .AddTeamPerformance()
                .AddCodeQuality()
                .AddProjectHealth()
                .AddWorkflowAnalysis()
                .AddRiskAnalysis()
                .AddCommitStatAnalysis();
        }

        private CommitAnalysisBuilder AddAnalysis(ICommitAnalysis analysis)
        {
            analyses.Add(analysis);
            return this;
        }

        public CommitAnalysisResult Build()
        {
            var result = new CommitAnalysisResult();

            foreach (var analysis in analyses)
            {
                analysis.Analyze(commits);

                switch (analysis)
                {
                    case CommitSizeAnalysis basic:
                        result.BasicMetrics = basic;
                        break;
                    case TeamPerformanceAnalysis team:
                        result.TeamPerformance = team;
                        break;
                    case CodeQualityAnalysis quality:
                        result.CodeQuality = quality;
                        break;
                    case ProjectHealthAnalysis health:
                        result.ProjectHealth = health;
                        break;
                    case WorkflowAnalysis workflow:
                        result.Workflow = workflow;
                        break;
                    case RiskAnalysis risk:
                        result.RiskIndicators = risk;
                        break;
                    case CommitStatAnalysis statAnalysis:
                        result.CommitStatAnalysis = statAnalysis;
                        break;
                }
            }

            return result;
        }
    }
}
