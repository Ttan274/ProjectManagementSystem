using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.Team.Dto;
using ProjectManagementSystem.Controllers.Base;

namespace ProjectManagementSystem.Controllers
{
    public class TeamController(ITeamService teamService) : BaseController
    {
        public async Task<IActionResult> GetTeamMemberPerformances(FilterTeamMemberPerformanceDto filterRequest)
        {
            if (filterRequest == null)
            {
                return BadRequest(Error("Invalid Request"));
            }

            try
            {
                var response = await teamService
                    .GetTeamMemberPerformancesAsync(filterRequest)
                    .ConfigureAwait(false);

                if (!response.Success)
                {
                    return Ok(response);
                }

                return BadRequest(response);
            }
            catch (Exception)
            {
                return BadRequest(Error("Internal server error occured"));
            }
        }
    }
}
