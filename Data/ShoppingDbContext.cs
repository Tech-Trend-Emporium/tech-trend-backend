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
            mb.Entity<Cart>().ToTable("Carts");
            mb.Entity<CartItem>().ToTable("CartItems");
            mb.Entity<WishList>().ToTable("WishLists");
            mb.Entity<WishListItem>().ToTable("WishListItems");

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
        }
    }
}
