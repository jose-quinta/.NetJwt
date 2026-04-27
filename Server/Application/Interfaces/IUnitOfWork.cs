using System.Linq.Expressions;

namespace Server.Application.Interfaces {
    public interface IUnitOfWork {
        IRepository<T> GetRepository<T>() where T : class;
        Task SaveAsync();
        void Dispose();
    }
}