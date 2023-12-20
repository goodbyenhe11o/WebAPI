using igs_backend.Models;

namespace igs_backend.Repository
{
    public interface IAnnounceRepository
    {

        void Add(Announce announce, string contextName);

        Task<IEnumerable<Announce>> GetAll(string contextName, string? language);

        Task<Announce> Get(string language, string contextName);

        //Task<Announce> Add(Announce announce, string contextName);
        //Task<Announce> Update(Announce announce, string contextName);
     

        //void Update(Announce announce, string contextName);

        void Delete(Announce announce, string contextName);
    }
}
