using ProjectManagementSystem.Application.Abstractions.Repositories.AppInfo;
using ProjectManagementSystem.Persistance.DbContext;

namespace ProjectManagementSystem.Persistance.Repositories.AppInfo
{
    public class AppInfoReadRepository(AppDbContext context)
        : ReadRepository<Domain.Entities.AppInfo>(context), IAppInfoReadRepository
    {
    }
}
