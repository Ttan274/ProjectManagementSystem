using ProjectManagementSystem.Application.Abstractions.Repositories.Project;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Project
{
    public class ProjectReadRepository : ReadRepository<Domain.Entities.Project>, IProjectReadRepository
    {
        public ProjectReadRepository(AppDbContext context) : base(context)
        {
        }
    }
}
