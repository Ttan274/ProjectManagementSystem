using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Repositories.Team;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.Team.Dto;
using ProjectManagementSystem.Application.Abstractions.User;
using System.Security.Claims;

namespace ProjectManagementSystem.Application.Team
{
    public class TeamService : ITeamService
    {
        private readonly ITeamReadRepository _teamReadRepository;
        private readonly ITeamWriteRepository _teamWriteRepository;
        private readonly IProjectService _projectService;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public TeamService(ITeamReadRepository teamReadRepository, ITeamWriteRepository teamWriteRepository, 
            IProjectService projectService, IUserService userService, IMapper mapper)
        {
            _teamReadRepository = teamReadRepository;
            _teamWriteRepository = teamWriteRepository;
            _projectService = projectService;
            _userService = userService;
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
                var teams = await _teamReadRepository.GetQueryable().Include(x => x.Projects).Where(x => x.Status).ToListAsync();

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

        public async Task<TeamDto> GetTeam(ClaimsPrincipal principal)
        {
            try
            {
                var teamId = await _userService.GetTeamId(principal);
                
                var id = Guid.Parse(teamId);

                var team = await _teamReadRepository.GetByIdAsync(id);

                var mappedResult = _mapper.Map<Domain.Entities.Team, TeamDto>(team);

                mappedResult.Projects = await _projectService.GetAllProjectsByTeamId(id);

                mappedResult.Users = await _userService.GetAllUsersByTeamId(id);

                return mappedResult;
            }
            catch (Exception)
            {
                return new TeamDto();
            }
        }
    }
}
