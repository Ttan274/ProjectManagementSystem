using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Application.Abstractions.Repositories
{
    public interface IWriteRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<bool> AddAsync(T entity);
        bool Remove(T entity);
        Task<bool> RemoveAsync(Guid id);
        bool Update(T entity);
        Task<int> SaveAsync();
    }
}
