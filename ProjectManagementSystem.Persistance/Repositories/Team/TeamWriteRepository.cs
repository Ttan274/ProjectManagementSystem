using ProjectManagementSystem.Application.Abstractions.Repositories.Team;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Team
{
    public class TeamWriteRepository : WriteRepository<Domain.Entities.Team>, ITeamWriteRepository
    {
        public TeamWriteRepository(AppDbContext context) : base(context)
        {
        }
    }
}
