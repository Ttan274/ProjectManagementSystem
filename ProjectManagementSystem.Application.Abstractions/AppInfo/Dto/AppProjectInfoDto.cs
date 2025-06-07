using ProjectManagementSystem.Application.Abstractions.Project.Dto;

namespace ProjectManagementSystem.Application.Abstractions.AppInfo.Dto
{
    public class AppProjectInfoDto
    {
        public AppProjectInfoDto()
        {
            AppInfos = [];
            Project = new();
        }

        public ProjectDto Project { get; set; }
        public List<AppInfoDto> AppInfos { get; set; }
    }
}
