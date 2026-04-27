using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Server.Application.Interfaces;
using Server.Domain.Entities;

namespace Server.Infrastructure.Persistence {
    public class Repository<T> : IRepository<T> where T : class {
        protected readonly DbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(DbContext context) {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public virtual async Task<T?> GetAsync(int id) => await _dbSet.FindAsync(id);
        public virtual async Task<IEnumerable<T>> GetAsync() => await _dbSet.ToListAsync();
        public virtual async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate) => await _dbSet.FirstOrDefaultAsync(predicate);
        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate) => await _dbSet.Where(predicate).ToListAsync();
        public virtual async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        #pragma warning disable CS1998
        public virtual async Task UpdateAsync(T entity) => _dbSet.Update(entity);
        public virtual async Task DeleteAsync(T entity) => _dbSet.Remove(entity);
        #pragma warning restore CS1998
        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}