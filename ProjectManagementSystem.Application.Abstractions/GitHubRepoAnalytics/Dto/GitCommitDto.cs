namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public class GitCommitDto
    {
        public GitCommitDto()
        {
            FilesChanged = [];
            FilesChangedInfos = [];
        }
        public string? Sha { get; set; }
        public string? Committer { get; set; }
        public int Additions { get; set; }
        public int Deletions { get; set; }
        public string? Message { get; set; }
        public List<string> FilesChanged { get; set; }
        private List<GitCommitFileDto> FilesChangedInfos { get; set; }
        public DateTime? Date { get; set; }
        public List<GitCommitFileDto> GetFilesChangedInfos()
        {
            return FilesChangedInfos;
        }
        public void AddFileChangedInfo(GitCommitFileDto info)
        {
            FilesChangedInfos.Add(info);
        }
    }
}
