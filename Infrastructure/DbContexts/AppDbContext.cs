using Data.Entities;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DbContexts
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<User> Users => Set<User>();
        public DbSet<Session> Sessions => Set<Session>();
        public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
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
        public DbSet<RecoveryQuestion> RecoveryQuestions => Set<RecoveryQuestion>();

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // --- Table names ---
            mb.Entity<User>().ToTable("users");
            mb.Entity<Session>().ToTable("sessions");
            mb.Entity<RefreshToken>().ToTable("refresh_tokens");
            mb.Entity<Category>().ToTable("categories");
            mb.Entity<Product>().ToTable("products");
            mb.Entity<Inventory>().ToTable("inventories");
            mb.Entity<Review>().ToTable("reviews");
            mb.Entity<Cart>().ToTable("carts");
            mb.Entity<CartItem>().ToTable("cart_items");
            mb.Entity<WishList>().ToTable("wish_lists");
            mb.Entity<WishListItem>().ToTable("wish_list_items");
            mb.Entity<Coupon>().ToTable("coupons");
            mb.Entity<ApprovalJob>().ToTable("approval_jobs");
            mb.Entity<RecoveryQuestion>().ToTable("recovery_questions");

            // --- Enum conversions ---
            mb.Entity<User>()
              .Property(u => u.Role)
              .HasConversion<int>();

            // Cart.Status is required (non-nullable enum)
            mb.Entity<Cart>()
              .Property(c => c.Status)
              .HasConversion<int>()
              .IsRequired();

            // These are OPTIONAL until checkout (nullable enums -> int?)
            mb.Entity<Cart>()
              .Property(c => c.PaymentMethod)
              .HasConversion<int?>()
              .IsRequired(false);

            mb.Entity<Cart>()
              .Property(c => c.PaymentStatus)
              .HasConversion<int?>()
              .IsRequired(false);

            // --- Users ---
            mb.Entity<User>().HasIndex(u => u.Email).IsUnique();
            mb.Entity<User>().HasIndex(u => u.Username).IsUnique();

            // Optional RecoveryQuestion on User (N:1)
            mb.Entity<User>()
                .HasOne(u => u.RecoveryQuestion)
                .WithMany()
                .HasForeignKey(u => u.RecoveryQuestionId)
                .OnDelete(DeleteBehavior.Restrict)
                .IsRequired(false);

            // Ensure max length for the hashed answer (shadow or concrete property)
            mb.Entity<User>()
              .Property<string?>("RecoveryAnswerHash")
              .HasMaxLength(256);

            // --- Sessions & Tokens ---
            mb.Entity<Session>()
              .HasOne(s => s.User)
              .WithMany(u => u.Sessions)
              .HasForeignKey(s => s.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<RefreshToken>()
                .HasOne(rt => rt.Session)
                .WithMany()
                .HasForeignKey(rt => rt.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

            mb.Entity<RefreshToken>().HasIndex(rt => rt.Token).IsUnique();

            // --- Catalogs / Products ---
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

            // --- Reviews ---
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

            // --- Coupons ---
            mb.Entity<Coupon>().HasIndex(c => c.Code).IsUnique();
            mb.Entity<Coupon>().Property(c => c.Discount).HasPrecision(18, 2);

            // --- Carts (User 1:N Carts) ---
            mb.Entity<User>()
              .HasMany(u => u.Carts)
              .WithOne(c => c.User)
              .HasForeignKey(c => c.UserId)
              .OnDelete(DeleteBehavior.Cascade);

            // Non-unique index on UserId + composite for queries
            mb.Entity<Cart>().HasIndex(c => c.UserId);
            mb.Entity<Cart>().HasIndex(c => new { c.UserId, c.Status, c.CreatedAt });

            // Cart -> Coupon (N:1)
            mb.Entity<Cart>()
              .HasOne(c => c.Coupon)
              .WithMany(cu => cu.Carts)
              .HasForeignKey(c => c.CouponId)
              .OnDelete(DeleteBehavior.SetNull);

            // Optional cart fields until checkout
            mb.Entity<Cart>()
              .Property(c => c.Address)
              .HasMaxLength(120)
              .IsRequired(false);

            mb.Entity<Cart>()
              .Property(c => c.TotalAmount)
              .HasPrecision(18, 2)
              .IsRequired(false);

            // --- Cart Items ---
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

            // --- Wish Lists ---
            mb.Entity<User>()
              .HasOne(u => u.WishList)
              .WithOne(w => w.User)
              .HasForeignKey<WishList>(w => w.UserId)
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

            // --- Approval Jobs ---
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

            // --- Common precisions / indexes ---
            mb.Entity<Product>().Property(p => p.Price).HasPrecision(18, 2);

            // --- Recovery Questions ---
            mb.Entity<RecoveryQuestion>().HasIndex(rq => rq.Question).IsUnique();
        }
    }
}
