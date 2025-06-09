namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class GitCommitFileDto
    {
        public GitCommitFileDto()
        {

        }
        public string? Filename { get; private set; }
        public int Additions { get; private set; }
        public int Deletions { get; private set; }
        public int Changes { get; private set; }
        public string? Status { get; private set; }
        public string? Patch { get; private set; }
        public string Extension => Path.GetExtension(Filename ?? "").ToLowerInvariant();

        public bool IsTestFile => Filename?.Contains("test", StringComparison.OrdinalIgnoreCase) ?? false;

        public bool IsDocFile => Filename?.Contains("docs", StringComparison.OrdinalIgnoreCase) == true
                                 || Extension == ".md";
    }
}
