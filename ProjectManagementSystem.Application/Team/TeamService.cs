using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Repositories.Team;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.Team.Dto;

namespace ProjectManagementSystem.Application.Team
{
    public class TeamService : ITeamService
    {
        private readonly ITeamReadRepository _teamReadRepository;
        private readonly ITeamWriteRepository _teamWriteRepository;
        private readonly IMapper _mapper;

        public TeamService(ITeamReadRepository teamReadRepository, ITeamWriteRepository teamWriteRepository, IMapper mapper)
        {
            _teamReadRepository = teamReadRepository;
            _teamWriteRepository = teamWriteRepository;
            _mapper = mapper;
        }

        public async Task<bool> CreateTeam(TeamDto team)
        {
            if (team is null)
                return false;

            try
            {
                var mappedResult = _mapper.Map<TeamDto, Domain.Entities.Team>(team);

                var response = await _teamWriteRepository.AddAsync(mappedResult);

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
                var response = await _teamWriteRepository.RemoveAsync(id);

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
                var teams = await _teamReadRepository.GetQueryable().Where(x => x.Status).ToListAsync();

                if (teams is null)
                    return [];

                var mappedResult = _mapper.Map<List<Domain.Entities.Team>,  List<TeamDto>>(teams);

                return mappedResult;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<TeamDto> GetTeamById(Guid id)
        {
            if (id == Guid.Empty)
                return new TeamDto();

            try
            {
                var team = await _teamReadRepository.GetByIdAsync(id);

                var mappedResult = _mapper.Map<Domain.Entities.Team, TeamDto>(team);

                return mappedResult;
            }
            catch (Exception)
            {
                return new TeamDto();
            }
        }
    }
}
