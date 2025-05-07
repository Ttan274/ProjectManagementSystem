using ProjectManagementSystem.Application.Abstractions.Repositories.Task;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Task
{
    public class TaskReadRepository : ReadRepository<Domain.Entities.Task>, ITaskReadRepository
    {
        public TaskReadRepository(AppDbContext context) : base(context)
        {
        }
    }
}
