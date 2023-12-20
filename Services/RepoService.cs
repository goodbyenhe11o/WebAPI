using igs_backend.Repository;
using Microsoft.EntityFrameworkCore;
using igs_backend.Models;
using igs_backend.Data;
using System.Linq.Expressions;

namespace igs_backend.Services
{
    public class RepoService<T>: ITRepository<T> where T : class
    {
        DbContextFactory _contexts;
        public List<T> contextSet;
        public BaseDbContext context;
        private readonly DbSet<T> _dbSet;

        public RepoService(DbContextFactory contexts)
        {
            _contexts = contexts;
           
        }

        public async Task CreateAsync(T entity, string contextName)
        {
            context = _contexts.GetContext(contextName);
            await context.AddAsync(entity);
           await context.SaveChangesAsync();
           
        }

        public async Task DeleteAsync(T entity, string contextName)
        {
            context = _contexts.GetContext(contextName);
            context.Set<T>().Remove(entity);
            await context.SaveChangesAsync();
        }

        public async Task<IEnumerable<T>> GetAllAsync(string contextName)
        {
            context = _contexts.GetContext(contextName);
            contextSet = await context.Set<T>().ToListAsync();
            return contextSet;
        }

        public async Task<T> GetByIdAsync(int id, string contextName)
        {
            context = _contexts.GetContext(contextName);
            return await context.Set<T>().FindAsync(id);
           
        }

        public async Task UpdateAsync(T entity, string contextName, Expression<Func<T, object>>[] updateProperties)
        {
            context = _contexts.GetContext(contextName);
            

            if (updateProperties != null)
            {
                foreach (var property in updateProperties)
                {
                    context.Entry<T>(entity).Property(property).IsModified = true;
                }

            await   context.SaveChangesAsync();
            }
        }
    }
}
