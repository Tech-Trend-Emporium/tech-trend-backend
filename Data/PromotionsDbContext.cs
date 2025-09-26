using Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class PromotionsDbContext(DbContextOptions<PromotionsDbContext> options) : DbContext(options)
    {
        public DbSet<Coupon> Coupons => Set<Coupon>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            mb.Entity<Coupon>().ToTable("Coupons");
        }
    }
}
