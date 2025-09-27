using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.DbContexts
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<WishList> WishLists => Set<WishList>();
        public DbSet<WishListItem> WishListItems => Set<WishListItem>();
        public DbSet<Coupon> Coupons => Set<Coupon>();
        public DbSet<ApprovalJob> ApprovalJobs => Set<ApprovalJob>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<User>().ToTable("Users");
            mb.Entity<Session>().ToTable("Sessions");
            mb.Entity<Category>().ToTable("Categories");
            mb.Entity<Product>().ToTable("Products");
            mb.Entity<Inventory>().ToTable("Inventories");
            mb.Entity<Review>().ToTable("Reviews");
            mb.Entity<Cart>().ToTable("Carts");
            mb.Entity<CartItem>().ToTable("CartItems");
            mb.Entity<WishList>().ToTable("WishLists");
            mb.Entity<WishListItem>().ToTable("WishListItems");
            mb.Entity<Coupon>().ToTable("Coupons");
            mb.Entity<ApprovalJob>().ToTable("ApprovalJobs");

            mb.Entity<Session>()
              .HasOne(s => s.User)
              .WithMany(u => u.Sessions)
              .HasForeignKey(s => s.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
            mb.Entity<User>().HasIndex(u => u.Username).IsUnique();

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

            mb.Entity<Review>()
              .HasOne(r => r.Product)
              .WithMany(p => p.Reviews)
              .HasForeignKey(r => r.ProductId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<Review>()
              .HasOne(r => r.User)
              .WithMany(u => u.Reviews)
              .HasForeignKey(r => r.UserId)
              .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<Review>().HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();

            mb.Entity<Coupon>().HasIndex(c => c.Code).IsUnique();
            mb.Entity<Coupon>().Property(c => c.Discount).HasPrecision(18, 2);

            mb.Entity<Cart>()
              .HasOne(c => c.User)
              .WithMany()
              .HasForeignKey(c => c.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<Cart>()
              .HasOne(c => c.Coupon)
              .WithMany()
              .HasForeignKey(c => c.CouponId)
              .OnDelete(DeleteBehavior.SetNull);

            mb.Entity<CartItem>()
              .HasOne(ci => ci.Cart)
              .WithMany(c => c.Items)
              .HasForeignKey(ci => ci.CartId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<CartItem>()
              .HasOne(ci => ci.Product)
              .WithMany(p => p.CartItems)
              .HasForeignKey(ci => ci.ProductId)
              .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<CartItem>().HasIndex(ci => new { ci.CartId, ci.ProductId }).IsUnique();

            mb.Entity<WishList>()
              .HasOne(w => w.User)
              .WithMany()
              .HasForeignKey(w => w.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<WishList>().HasIndex(w => w.UserId).IsUnique();

            mb.Entity<WishListItem>()
              .HasOne(wi => wi.WishList)
              .WithMany(w => w.Items)
              .HasForeignKey(wi => wi.WishListId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<WishListItem>()
              .HasOne(wi => wi.Product)
              .WithMany(p => p.WishListItems)
              .HasForeignKey(wi => wi.ProductId)
              .OnDelete(DeleteBehavior.Restrict);

            mb.Entity<WishListItem>().HasIndex(wi => new { wi.WishListId, wi.ProductId }).IsUnique();

            mb.Entity<ApprovalJob>()
                .HasOne(j => j.RequestedByUser)
                .WithMany(u => u.RequestedJobs)
                .HasForeignKey(j => j.RequestedBy)
                .OnDelete(DeleteBehavior.Restrict)  
                .IsRequired();

            mb.Entity<ApprovalJob>()
                .HasOne(j => j.DecidedByUser)
                .WithMany(u => u.DecidedJobs)
                .HasForeignKey(j => j.DecidedBy)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            mb.Entity<ApprovalJob>().HasIndex(x => new { x.State, x.RequestedAt });
            mb.Entity<ApprovalJob>().HasIndex(x => x.RequestedBy);
            mb.Entity<ApprovalJob>().HasIndex(x => x.DecidedBy);

            mb.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);
        }
    }
}
