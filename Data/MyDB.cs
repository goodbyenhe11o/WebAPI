using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using igs_backend.Models;

namespace igs_backend.Data
{
    public class MyDB : BaseDbContext
    {
        public MyDB (DbContextOptions<MyDB> options)
            : base(options)
        {
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


    }
}
