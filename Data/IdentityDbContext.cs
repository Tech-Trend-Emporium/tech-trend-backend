using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Session> Sessions => Set<Session>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<User>().ToTable("Users");
            mb.Entity<Session>().ToTable("Sessions");

            mb.Entity<Session>()
              .HasOne(s => s.User)
              .WithMany(u => u.Sessions)
              .HasForeignKey(s => s.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<User>().Ignore(u => u.Reviews);
            mb.Entity<User>().Ignore(u => u.RequestedJobs);
            mb.Entity<User>().Ignore(u => u.DecidedJobs);
            mb.Entity<User>().Ignore(u => u.WishList);
            mb.Entity<User>().Ignore(u => u.Cart);
        }
    }
}
