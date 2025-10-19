using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;

namespace Application.Repository
{
    /// <summary>
    /// Repository implementation for managing <see cref="Review"/> entities.
    /// Inherits base CRUD functionality from <see cref="EfRepository{Review}"/>.
    /// This class is documented by AI.
    /// </summary>
    public class ReviewRepository : EfRepository<Review>, IReviewRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ReviewRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for database access.</param>
        public ReviewRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        // This repository currently relies entirely on the generic EfRepository<T> implementation.
        // All standard CRUD and query methods are inherited.
        // You can add custom review-specific queries or operations here in the future if needed.
    }
}
