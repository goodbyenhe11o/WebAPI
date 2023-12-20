using System.Linq.Expressions;

namespace igs_backend.Repository
{
    public interface ITRepository<T>
    {
        Task<T> GetByIdAsync(int id, string contextName);
        Task<IEnumerable<T>> GetAllAsync(string contextName);
        Task CreateAsync(T entity, string contextName);
        Task UpdateAsync(T entity, string contextName, Expression<Func<T, object>>[] updateProperties);
        Task DeleteAsync(T entity, string contextName);

    }
}
