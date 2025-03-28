using ProjectManagementSystem.Domain.Common;
using System.Linq.Expressions;

namespace ProjectManagementSystem.Application.Abstractions.Repositories
{
    public interface IReadRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<T> GetFirstOrDefaultAsync(Expression<Func<T, bool>> method, bool tracking = true);
        Task<T> GetByIdAsync(Guid id, bool tracking = true);
        IQueryable<T> GetQueryable(bool tracking = true);
        IQueryable<T> GetWhere(Expression<Func<T, bool>> method, bool tracking = true);

    }
}
