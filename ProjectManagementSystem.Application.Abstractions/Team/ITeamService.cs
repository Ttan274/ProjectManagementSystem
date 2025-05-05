using ProjectManagementSystem.Application.Abstractions.Team.Dto;
using System.Security.Claims;

namespace ProjectManagementSystem.Application.Abstractions.Team
{
    public interface ITeamService
    {
        Task<bool> CreateTeam(TeamDto team);
        Task<bool> DeleteTeam(Guid id);
        Task<TeamDto> GetTeam(ClaimsPrincipal principal); 
        Task<List<TeamDto>> GetAllTeams();
    }
}
