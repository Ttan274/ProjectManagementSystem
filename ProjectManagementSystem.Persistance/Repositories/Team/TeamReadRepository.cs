using ProjectManagementSystem.Application.Abstractions.Repositories.Team;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Team
{
    public class TeamReadRepository : ReadRepository<Domain.Entities.Team>, ITeamReadRepository
    {
        public TeamReadRepository(AppDbContext context) : base(context)
        {
        }
    }
}
