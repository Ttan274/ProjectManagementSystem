using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Repositories.Team;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.Team.Dto;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Common.Consts;
using ProjectManagementSystem.Common.ServiceResponse;
using System.Security.Claims;

namespace ProjectManagementSystem.Application.Team
{
    public class TeamService(
        IServiceResponseHelper serviceResponseHelper,
        ITeamReadRepository teamReadRepository,
        ITeamWriteRepository teamWriteRepository,
        IProjectService projectService,
        IUserService userService,
        IMapper mapper) : ITeamService
    {
        public async Task<bool> CreateTeam(TeamDto team)
        {
            if (team is null)
                return false;

            try
            {
                var mappedResult = mapper.Map<TeamDto, Domain.Entities.Team>(team);

                var response = await teamWriteRepository.AddAsync(mappedResult);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DeleteTeam(Guid id)
        {
            if (id == Guid.Empty)
                return false;

            try
            {
                var response = await teamWriteRepository.RemoveAsync(id);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<TeamDto>> GetAllTeams()
        {
            try
            {
                var teams = await teamReadRepository.GetQueryable().Include(x => x.Projects).Where(x => x.Status).ToListAsync();

                if (teams is null)
                    return [];

                var mappedResult = mapper.Map<List<Domain.Entities.Team>, List<TeamDto>>(teams);

                return mappedResult;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<TeamDto> GetTeamByUser(ClaimsPrincipal principal)
        {
            try
            {
                var teamId = await userService.GetTeamId(principal);

                var result = await GetTeam(Guid.Parse(teamId));

                return result;
            }
            catch (Exception)
            {
                return new TeamDto();
            }
        }

        public async Task<TeamDto> GetTeamById(Guid id)
        {
            try
            {
                var result = await GetTeam(id);

                return result;
            }
            catch (Exception)
            {
                return new TeamDto();
            }
        }

        //Utility method
        private async Task<TeamDto> GetTeam(Guid id)
        {
            var team = await teamReadRepository.GetByIdAsync(id);

            var mappedResult = mapper.Map<Domain.Entities.Team, TeamDto>(team);

            mappedResult.Projects = await projectService.GetAllProjectsByTeamId(id);

            mappedResult.Users = await userService.GetAllUsersByTeamId(id);

            return mappedResult;
        }

        public async Task<ServiceResponse<List<TeamMemberPerformanceDto>>> GetTeamMemberPerformancesAsync(FilterTeamMemberPerformanceDto filterDto)
        {
            if (filterDto == null)
            {
                return serviceResponseHelper.SetError<List<TeamMemberPerformanceDto>>("Invalid Request");
            }

            filterDto.Validate(out var validationResult);

            if (!string.IsNullOrWhiteSpace(validationResult))
            {
                return serviceResponseHelper.SetError<List<TeamMemberPerformanceDto>>(validationResult);
            }

            try
            {
                var startDate = filterDto.StartDate;
                var endDate = filterDto.EndDate;

                var teamMemberPerformances = await teamReadRepository
                    .GetQueryable(tracking: false)
                    .Where(team => team.Id == filterDto.TeamId)
                    .SelectMany(team => team.Users)
                    .Select(member => new
                    {
                        member.Id,
                        member.Name,
                        member.Tasks,
                        CompletedTasks = member.Tasks
                            .Where(task => task.State == Common.Enums.ProjectStatus.Done)
                            .Select(task => new
                            {
                                task.CompletedAt,
                                task.Sprint.CreatedDatee,
                                task.Sprint.FinishDate
                            })
                            .ToList(),
                        BugsResolved = member.Tasks.Count(task =>
                            task.Type == Common.Enums.TaskType.BugFix && task.State == Common.Enums.ProjectStatus.Done),
                        StoryPointsCompleted = member.Tasks
                            .Where(task => task.State == Common.Enums.ProjectStatus.Done)
                            .Select(task => task.EffortScore)
                            .Sum()
                    })
                    .ToListAsync();

                var result = teamMemberPerformances
                    .Select(member =>
                    {
                        int totalTasks = member.Tasks.Count;
                        int completedTasks = member.CompletedTasks.Count;

                        int taskCompletionRate = totalTasks > 0
                            ? (int)(100.0 * completedTasks / totalTasks)
                            : 0;

                        int onTimeDelivered = member.CompletedTasks.Count(task =>
                            task.CompletedAt.HasValue &&
                            task.CreatedDatee >= startDate &&
                            task.CreatedDatee <= endDate &&
                            task.CompletedAt <= task.FinishDate);

                        int onTimeDeliveryRate = completedTasks > 0
                            ? (int)(100.0 * onTimeDelivered / completedTasks)
                            : 0;

                        return new TeamMemberPerformanceDto
                        {
                            Id = member.Id,
                            Name = member.Name,
                            Role = Defaults.TEAM_MEMBER,
                            BugsResolved = member.BugsResolved,
                            StoryPointsCompleted = member.StoryPointsCompleted,
                            TaskCompletionRate = taskCompletionRate,
                            OnTimeDeliveryRate = onTimeDeliveryRate,
                            StatusBadge = GetStatusBadge(taskCompletionRate, onTimeDeliveryRate)
                        };
                    })
                    .ToList();

                return serviceResponseHelper.SetSuccess(result);
            }
            catch (Exception)
            {
                return serviceResponseHelper.SetError(new List<TeamMemberPerformanceDto>(), "Internal server error");
            }
        }
        private static string GetStatusBadge(int taskCompletionRate, int onTimeDeliveryRate)
        {
            if (taskCompletionRate >= 90 && onTimeDeliveryRate >= 90) return "Excellent";
            if (taskCompletionRate >= 75 && onTimeDeliveryRate >= 75) return "Good";
            if (taskCompletionRate >= 50) return "Fair";
            return "Needs Improvement";
        }
    }
}
