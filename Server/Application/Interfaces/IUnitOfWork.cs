using System.Linq.Expressions;
using Server.Domain.Entities;

namespace Server.Application.Interfaces {
    public interface IRepository<T> where T : class {
        Task<T?> GetAsync(int id);
        Task<IEnumerable<T>> GetAsync();
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }

    public interface IUnitOfWork {
        IRepository<T> GetRepository<T>() where T : class;
        Task SaveAsync();
        void Dispose();
    }

    public interface IAuthService {
        Task<(User? User, string? Error)> RegisterAsync(DTOs.RegisterRequest request);
        Task<(DTOs.AuthResponse? Response, string? Error)> LoginAsync(DTOs.LoginRequest request);
        Task<(DTOs.AuthResponse? Response, string? Error)> RefreshTokenAsync(DTOs.RefreshTokenRequest request);
    }
}