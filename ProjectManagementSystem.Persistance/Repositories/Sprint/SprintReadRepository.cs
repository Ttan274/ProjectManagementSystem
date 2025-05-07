using ProjectManagementSystem.Application.Abstractions.Repositories.Sprint;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Sprint
{
    public class SprintReadRepository : ReadRepository<Domain.Entities.Sprint>, ISprintReadRepository
    {
        public SprintReadRepository(AppDbContext context) : base(context)
        {
        }
    }
}
