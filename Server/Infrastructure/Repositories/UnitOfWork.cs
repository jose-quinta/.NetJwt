using System.Collections.Concurrent;
using Server.Application.Interfaces;

namespace Server.Infrastructure.Persistence {
    public class UnitOfWork : IUnitOfWork {
        private readonly ApplicationDbContext _context;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context) => _context = context;

        public IRepository<T> GetRepository<T>() where T : class {
            return (IRepository<T>)_repositories.GetOrAdd(
                typeof(T),
                _ => new Repository<T>(_context));
        }

        public async Task SaveAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}