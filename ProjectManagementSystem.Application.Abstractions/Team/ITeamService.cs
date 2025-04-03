using ProjectManagementSystem.Application.Abstractions.Team.Dto;

namespace ProjectManagementSystem.Application.Abstractions.Team
{
    public interface ITeamService
    {
        Task<TeamDto> GetTeamById(Guid id); 
        Task<List<TeamDto>> GetAllTeams();
    }
}
