using Microsoft.EntityFrameworkCore;
using igs_backend.Models;
using Microsoft.AspNetCore.Authentication;

namespace igs_backend.Data
{
    public class BaseDbContext : DbContext
    {
        public  BaseDbContext(DbContextOptions options): base(options) { 
        
        
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Permissions>().HasData(
       new Permissions { ID = 1, Name = "announcement" },
       new Permissions { ID = 2, Name = "banner" },
       new Permissions { ID = 3, Name = "policy" },
       new Permissions { ID = 4, Name = "groups" },
       new Permissions { ID = 5, Name = "logos" },
       new Permissions { ID = 6, Name = "marquee" },
       new Permissions { ID = 7, Name = "news" },
       new Permissions { ID = 8, Name = "theme" },
       new Permissions { ID = 9, Name = "users" },
       new Permissions { ID = 10, Name = "withdrawpolicy" },
       new Permissions { ID = 11, Name = "depositpolicy"},
       new Permissions { ID = 12, Name = "maintenances"}

   );

        }

        public DbSet<igs_backend.Models.Announce>? Announce { get; set; }

        public DbSet<igs_backend.Models.Banner>? Banner { get; set; }

        public DbSet<igs_backend.Models.Group>? Group { get; set; }

        public DbSet<igs_backend.Models.Maintenance>? Maintenance { get; set; }

        public DbSet<igs_backend.Models.News>? News { get; set; }

        public DbSet<igs_backend.Models.Marquee>? Marquee { get; set; }

        public DbSet<igs_backend.Models.Logo>? Logo { get; set; }

        public DbSet<igs_backend.Models.User>? User { get; set; }

        public DbSet<igs_backend.Models.DepositPolicy>? DepositPolicy { get; set; }

        public DbSet<igs_backend.Models.WithdrawPolicy>? WithdrawPolicy { get; set; }

        public DbSet<igs_backend.Models.Permissions>? Permissions { get; set; }
        public DbSet<igs_backend.Models.UserPermissions>? UserPermissions { get; set; }


    }
}
