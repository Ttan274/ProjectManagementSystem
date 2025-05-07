using ProjectManagementSystem.Application.Abstractions.Repositories.Task;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.Task
{
    public class TaskWriteRepository : WriteRepository<Domain.Entities.Task>, ITaskWriteRepository
    {
        public TaskWriteRepository(AppDbContext context) : base(context)
        {
        }
    }
}
