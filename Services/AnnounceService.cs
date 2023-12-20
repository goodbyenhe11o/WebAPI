using igs_backend.Data;
using igs_backend.Models;
using igs_backend.Repository;
using Microsoft.EntityFrameworkCore;
using System;

namespace igs_backend.Services
{
    public class AnnounceService : IAnnounceRepository
    {
        DbContextFactory _contexts;

        public AnnounceService(DbContextFactory contexts)
        {
            _contexts = contexts;
        }


        public void Add(Announce announce, string contextName)
        {
            var context = _contexts.GetContext(contextName);
            var addedEntity = context.Attach(announce);
            addedEntity.State = EntityState.Added;
            context.SaveChanges();
        }

        public async Task<Announce> Get(string language, string contextName)
        {
            var context = _contexts.GetContext(contextName);
           
            return await context.Announce
                .FirstOrDefaultAsync(e => e.Language == language);
        }

        public async Task<IEnumerable<Announce>> GetAll(string contextName, string? language)
        {
            var context = _contexts.GetContext(contextName); //find the db;
            List<Announce> contextSet = null;
            if (!string.IsNullOrEmpty(language))
            {
                contextSet = await context.Set<Announce>().Where(x => x.Language == language).ToListAsync();

            }
            else
            {
                contextSet = await context.Set<Announce>().ToListAsync();
            }

            return contextSet;
        }

        public void Delete(Announce announce, string contextName)
        {
            var context = _contexts.GetContext(contextName);
            context.Entry<Announce>(announce).State = EntityState.Deleted;
        }

        public void Update(Announce announce, string contextName)
        {
            var context = _contexts.GetContext(contextName);
            var updatedEntity = context.Entry(announce);
            updatedEntity.State = EntityState.Modified;
            context.SaveChanges();
        }

       
    }
}
