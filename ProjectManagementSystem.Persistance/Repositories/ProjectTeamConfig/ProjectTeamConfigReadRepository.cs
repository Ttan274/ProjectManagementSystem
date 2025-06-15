using ProjectManagementSystem.Application.Abstractions.Repositories.ProjectTeamConfig;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.ProjectTeamConfig
{
    public class ProjectTeamConfigReadRepository(AppDbContext context)
        : ReadRepository<Domain.Entities.ProjectTeamConfig>(context), IProjectTeamConfigReadRepository
    {
    }
}
