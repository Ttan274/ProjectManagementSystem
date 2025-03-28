using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ProjectManagementSystem.Application.Abstractions.Repositories;
using ProjectManagementSystem.Domain.Common;
using ProjectManagementSystem.Persistance.DbContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Persistance.Repositories
{
    public class WriteRepository<T> : IWriteRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;

        public WriteRepository(AppDbContext context)
        {
            _context = context;
        }

        public DbSet<T> Table => _context.Set<T>();

        public async Task<bool> AddAsync(T entity)
        {
            EntityEntry<T> entry = await Table.AddAsync(entity);
            var result = entry.State == EntityState.Added;
            await SaveAsync();
            return result;
        }

        public bool Remove(T entity)
        {
            EntityEntry<T> entry = Table.Remove(entity);
            var result = entry.State == EntityState.Deleted;
            Save();
            return result;
        }

        public async Task<bool> RemoveAsync(Guid id)
        {
            T entity = await Table.FirstOrDefaultAsync(x => x.Id == id);
            var result = Remove(entity);
            await SaveAsync();
            return result;
        }

        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();

        public int Save() => _context.SaveChanges();

        public bool Update(T entity)
        {
            EntityEntry<T> entry = Table.Update(entity);
            var result = entry.State == EntityState.Modified;
            Save();
            return result;
        }
    }

}
