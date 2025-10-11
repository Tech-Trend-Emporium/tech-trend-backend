using Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Data
{
    public class PromotionsDbContext(DbContextOptions<PromotionsDbContext> options) : DbContext(options)
    {
        public DbSet<Coupon> Coupons => Set<Coupon>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Coupon>().ToTable("coupons");

            mb.Ignore<Cart>();
            mb.Ignore<CartItem>();
            mb.Ignore<Product>();
            mb.Ignore<Category>();
            mb.Ignore<Inventory>();
            mb.Ignore<Review>();
            mb.Ignore<User>();
            mb.Ignore<Session>();
            mb.Ignore<ApprovalJob>();
            mb.Ignore<WishList>();
            mb.Ignore<WishList>();   
            mb.Ignore<WishListItem>();

            mb.Entity<Coupon>().Ignore(c => c.Carts);

            mb.Entity<Coupon>(e =>
            {
                e.HasIndex(x => x.Code).IsUnique();

                e.Property(x => x.Discount).HasPrecision(18, 2);

                e.Property(x => x.ValidFrom).HasDefaultValueSql("now() at time zone 'utc'");
                e.Property(x => x.Active).HasDefaultValue(true);
            });
        }
    }
}
