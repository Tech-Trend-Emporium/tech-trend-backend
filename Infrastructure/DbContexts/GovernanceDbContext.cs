using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class GovernanceDbContext(DbContextOptions<GovernanceDbContext> options) : DbContext(options)
    {
        public DbSet<ApprovalJob> ApprovalJobs => Set<ApprovalJob>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<ApprovalJob>().ToTable("ApprovalJobs");

            mb.Ignore<User>();
            mb.Ignore<Session>();
            mb.Ignore<Product>();
            mb.Ignore<Category>();
            mb.Ignore<Inventory>();
            mb.Ignore<Review>();
            mb.Ignore<Cart>();
            mb.Ignore<CartItem>();
            mb.Ignore<WishList>();     
            mb.Ignore<WishList>();     
            mb.Ignore<WishListItem>();
            mb.Ignore<WishListItem>();
            mb.Ignore<Coupon>();

            mb.Entity<ApprovalJob>().Ignore(j => j.RequestedByUser);
            mb.Entity<ApprovalJob>().Ignore(j => j.DecidedByUser);

            mb.Entity<ApprovalJob>(e =>
            {
                e.Property(x => x.Type).HasConversion<int>();
                e.Property(x => x.Operation).HasConversion<int>();

                e.HasIndex(x => new { x.State, x.RequestedAt });
                e.HasIndex(x => x.RequestedBy);
                e.HasIndex(x => x.DecidedBy);

                e.Property(x => x.RequestedAt).HasDefaultValueSql("now() at time zone 'utc'");
            });
        }
    }
}
