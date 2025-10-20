using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class IdentityDbContext(DbContextOptions<IdentityDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Session> Sessions => Set<Session>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<User>().ToTable("users");
            mb.Entity<Session>().ToTable("sessions");

            mb.Ignore<Product>();
            mb.Ignore<Category>();
            mb.Ignore<Inventory>();
            mb.Ignore<Review>();
            mb.Ignore<Cart>();
            mb.Ignore<CartItem>();
            mb.Ignore<WishList>();     
            mb.Ignore<WishList>();     
            mb.Ignore<WishListItem>();
            mb.Ignore<Coupon>();
            mb.Ignore<ApprovalJob>();

            mb.Entity<User>().Ignore(u => u.Reviews);
            mb.Entity<User>().Ignore(u => u.RequestedJobs);
            mb.Entity<User>().Ignore(u => u.DecidedJobs);
            mb.Entity<User>().Ignore(u => u.WishList);   
            mb.Entity<User>().Ignore(u => u.Cart);

            mb.Entity<Session>()
              .HasOne(s => s.User)
              .WithMany(u => u.Sessions)
              .HasForeignKey(s => s.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
            mb.Entity<User>().HasIndex(u => u.Username).IsUnique();

            mb.Entity<User>().Property(u => u.Role).HasConversion<int>();

            mb.Entity<User>().Property(u => u.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
            mb.Entity<Session>().Property(s => s.LoginAt).HasDefaultValueSql("now() at time zone 'utc'");
        }
    }
}
