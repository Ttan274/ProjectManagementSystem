using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Domain.Common;

namespace ProjectManagementSystem.Application.Abstractions.Repositories
{
    public interface IRepository<T> where T : BaseEntity
    {
        DbSet<T> Table {  get; }    
    }
}
