using ProjectManagementSystem.Application.Abstractions.Repositories.AppInfo;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.AppInfo
{
    public class AppInfoWriteRepository(AppDbContext context)
        : WriteRepository<Domain.Entities.AppInfo>(context), IAppInfoWriteRepository
    {
    }
}
