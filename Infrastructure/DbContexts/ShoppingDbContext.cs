using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class ShoppingDbContext(DbContextOptions<ShoppingDbContext> options) : DbContext(options)
    {
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<WishList> WishLists => Set<WishList>();
        public DbSet<WishListItem> WishListItems => Set<WishListItem>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Cart>().ToTable("carts");
            mb.Entity<CartItem>().ToTable("cart_items");
            mb.Entity<WishList>().ToTable("wish_lists");
            mb.Entity<WishListItem>().ToTable("wish_list_items");

            mb.Ignore<User>();        
            mb.Ignore<Session>();     
            mb.Ignore<Product>();     
            mb.Ignore<Category>();    
            mb.Ignore<Inventory>();   
            mb.Ignore<Review>();      
            mb.Ignore<Coupon>();      
            mb.Ignore<ApprovalJob>(); 

            mb.Entity<Cart>().Ignore(c => c.User);
            mb.Entity<Cart>().Ignore(c => c.Coupon);
            mb.Entity<WishList>().Ignore(w => w.User);
            mb.Entity<CartItem>().Ignore(ci => ci.Product);
            mb.Entity<WishListItem>().Ignore(wi => wi.Product);

            mb.Entity<CartItem>()
              .HasOne(ci => ci.Cart)
              .WithMany(c => c.Items)
              .HasForeignKey(ci => ci.CartId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<WishListItem>()
              .HasOne(wi => wi.WishList)
              .WithMany(w => w.Items)
              .HasForeignKey(wi => wi.WishListId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<CartItem>().HasIndex(ci => new { ci.CartId, ci.ProductId }).IsUnique();
            mb.Entity<WishListItem>().HasIndex(wi => new { wi.WishListId, wi.ProductId }).IsUnique();

            mb.Entity<WishList>().HasIndex(w => w.UserId).IsUnique();

            mb.Entity<Cart>().Property(c => c.CreatedAt).HasDefaultValueSql("now() at time zone 'utc'");
            mb.Entity<WishListItem>().Property(x => x.AddedAt).HasDefaultValueSql("now() at time zone 'utc'");
        }
    }
}
