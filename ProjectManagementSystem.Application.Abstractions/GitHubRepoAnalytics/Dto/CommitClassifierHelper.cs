using System.Text.RegularExpressions;

namespace ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics.Dto
{
    public static class CommitClassifierHelper
    {
        public static bool IsRefactor(GitCommitFileDto file)
        {
            if (string.IsNullOrEmpty(file.Patch)) return false;

            return Regex.IsMatch(file.Patch, @"[-+]\s*(public|private|var|int|if|else|return).*")
                && !file.Patch.Contains("fix", StringComparison.OrdinalIgnoreCase)
                && !file.Patch.Contains("bug", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsFeature(GitCommitFileDto file)
        {
            if (string.IsNullOrEmpty(file.Patch)) return false;

            int additionLines = file.Patch.Split('\n').Count(line => line.StartsWith("+") && !line.StartsWith("+++"));
            return additionLines > 20 || file.Status == "added";
        }

        public static bool IsBugfix(GitCommitFileDto file)
        {
            if (string.IsNullOrEmpty(file.Patch)) return false;

            return file.Patch.Contains("fix", StringComparison.OrdinalIgnoreCase)
                || file.Patch.Contains("bug", StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsDocChange(GitCommitFileDto file)
        {
            return file.IsDocFile || (file.Patch?.Contains("README", StringComparison.OrdinalIgnoreCase) ?? false);
        }
    }

}
