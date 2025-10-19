using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Application.Repository
{
    /// <summary>
    /// Repository implementation for managing <see cref="ApprovalJob"/> entities.
    /// Provides data access methods specific to approval job workflows.
    /// This class is documented by AI.
    /// </summary>
    public class ApprovalJobRepository : EfRepository<ApprovalJob>, IApprovalJobRepository
    {
        private readonly AppDbContext _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApprovalJobRepository"/> class.
        /// </summary>
        /// <param name="db">The application's <see cref="AppDbContext"/> used for database access.</param>
        public ApprovalJobRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        /// <summary>
        /// Retrieves an <see cref="ApprovalJob"/> entity by its unique identifier with Entity Framework tracking enabled.
        /// </summary>
        /// <param name="id">The unique identifier of the approval job.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the <see cref="ApprovalJob"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public Task<ApprovalJob?> GetByIdTrackedAsync(int id, CancellationToken ct = default)
        {
            return _db.ApprovalJobs.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        /// <summary>
        /// Retrieves a paginated list of approval jobs that are still pending (i.e., not yet decided).
        /// </summary>
        /// <param name="skip">The number of records to skip before collecting results.</param>
        /// <param name="take">The number of records to return.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of pending <see cref="ApprovalJob"/> entities.
        /// </returns>
        public async Task<IReadOnlyList<ApprovalJob>> ListPendingAsync(int skip, int take, CancellationToken ct = default)
        {
            return await _db.ApprovalJobs
                .Where(j => j.DecidedAt == null)
                .OrderByDescending(j => j.RequestedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync(ct);
        }
    }
}
