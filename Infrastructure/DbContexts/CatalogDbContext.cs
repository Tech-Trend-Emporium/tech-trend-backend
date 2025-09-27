using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class CatalogDbContext(DbContextOptions<CatalogDbContext> options) : DbContext(options)
    {
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Review> Reviews => Set<Review>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Product>().ToTable("Products");    
            mb.Entity<Category>().ToTable("Categories");
            mb.Entity<Inventory>().ToTable("Inventories");
            mb.Entity<Review>().ToTable("Reviews");

            mb.Ignore<User>();
            mb.Ignore<Cart>();
            mb.Ignore<CartItem>();
            mb.Ignore<WishList>();
            mb.Ignore<WishListItem>();
            mb.Ignore<Coupon>();
            mb.Ignore<ApprovalJob>();
            mb.Ignore<Session>();

            mb.Entity<Review>().Ignore(r => r.User);
            mb.Entity<Product>().Ignore(p => p.CartItems);
            mb.Entity<Product>().Ignore(p => p.WishListItems);

            mb.Entity<Product>()
              .HasOne(p => p.Category)
              .WithMany(c => c.Products)
              .HasForeignKey(p => p.CategoryId)
              .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<Product>()
              .HasOne(p => p.Inventory)
              .WithOne(i => i.Product)
              .HasForeignKey<Inventory>(i => i.ProductId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<Inventory>().HasIndex(i => i.ProductId).IsUnique();
            mb.Entity<Review>().HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();
            mb.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        }
    }
}
