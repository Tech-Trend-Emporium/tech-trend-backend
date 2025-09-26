using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<ApprovalJob> ApprovalJobs => Set<ApprovalJob>();
        public DbSet<Cart> Carts => Set<Cart>();
        public DbSet<CartItem> CartItems => Set<CartItem>();
        public DbSet<Category> Categories => Set<Category>();
        public DbSet<Coupon> Coupons => Set<Coupon>();
        public DbSet<Inventory> Inventories => Set<Inventory>();
        public DbSet<Product> Products => Set<Product>();
        public DbSet<Review> Reviews => Set<Review>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<User> Users => Set<User>();
        public DbSet<WishList> WishLists => Set<WishList>();
        public DbSet<WishListItem> WishListItems => Set<WishListItem>();
    }
}
