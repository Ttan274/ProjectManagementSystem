using ProjectManagementSystem.Application.Abstractions.Team.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Team
{
    public interface ITeamService
    {
        Task<bool> CreateTeam(TeamDto team);
        Task<bool> DeleteTeam(Guid id);
        Task<TeamDto> GetTeamById(Guid id); 
        Task<List<TeamDto>> GetAllTeams();
    }
}
