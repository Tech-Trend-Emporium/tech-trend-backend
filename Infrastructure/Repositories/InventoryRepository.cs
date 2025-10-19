using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;

namespace Application.Repository
{
    /// <summary>
    /// Repository implementation for managing <see cref="Inventory"/> entities.
    /// Inherits base CRUD functionality from <see cref="EfRepository{T}"/>.
    /// This class is documented by AI.
    /// </summary>
    public class InventoryRepository : EfRepository<Inventory>, IInventoryRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for database operations.</param>
        public InventoryRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        // Currently, this repository does not define any custom methods.
        // All standard CRUD operations are inherited from EfRepository<Inventory>.
        // Custom inventory-specific queries can be implemented here in the future.
    }
}
