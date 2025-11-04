using Application.Abstractions;
using Domain.Entities;
using Infrastructure.DbContexts;

namespace Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for managing <see cref="RecoveryQuestion"/> entities.
    /// Inherits base CRUD functionality from <see cref="EfRepository{T}"/>.
    /// This class is documented by AI.
    /// </summary>
    public class RecoveryQuestionRepository : EfRepository<RecoveryQuestion>, IRecoveryQuestionRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="RecoveryQuestionRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for database operations.</param>
        public RecoveryQuestionRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        // Currently, this repository does not define any custom methods.
        // All standard CRUD operations are inherited from EfRepository<RecoveryQuestion>.
        // Custom recovery question specific queries can be implemented here in the future.
    }
}
