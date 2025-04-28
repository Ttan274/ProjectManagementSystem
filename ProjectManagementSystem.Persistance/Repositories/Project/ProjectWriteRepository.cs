using ProjectManagementSystem.Application.Abstractions.Repositories.Project;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Project
{
    public class ProjectWriteRepository : WriteRepository<Domain.Entities.Project>, IProjectWriteRepository
    {
        public ProjectWriteRepository(AppDbContext context) : base(context)
        {
        }
    }
}
