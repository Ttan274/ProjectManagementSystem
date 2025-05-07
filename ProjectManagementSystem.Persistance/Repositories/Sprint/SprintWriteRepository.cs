using ProjectManagementSystem.Application.Abstractions.Repositories.Sprint;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Sprint
{
    public class SprintWriteRepository : WriteRepository<Domain.Entities.Sprint>, ISprintWriteRepository
    {
        public SprintWriteRepository(AppDbContext context) : base(context)
        {
        }
    }
}
