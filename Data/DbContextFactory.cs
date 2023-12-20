namespace igs_backend.Data
{
    public class DbContextFactory
    {
        private readonly IDictionary<string, BaseDbContext> _context;

        public DbContextFactory(IDictionary<string, BaseDbContext> context)
        {
            _context = context;
        }

        public BaseDbContext GetContext(string contextName)
        {
            return _context[contextName];
        }



    }
}
