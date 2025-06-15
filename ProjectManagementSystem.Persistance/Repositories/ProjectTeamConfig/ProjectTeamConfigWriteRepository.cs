using ProjectManagementSystem.Application.Abstractions.Repositories.ProjectTeamConfig;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.ProjectTeamConfig
{
    public class ProjectTeamConfigWriteRepository(AppDbContext context)
        : WriteRepository<Domain.Entities.ProjectTeamConfig>(context), IProjectTeamConfigWriteRepository
    {
    }
}
