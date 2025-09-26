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
        }
    }
}
