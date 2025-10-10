using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class ApprovalJobRepository : EfRepository<ApprovalJob>, IApprovalJobRepository
    {
        private readonly AppDbContext _db;

        public ApprovalJobRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public Task<ApprovalJob?> GetByIdTrackedAsync(int id, CancellationToken ct = default)
        {
            return _db.ApprovalJobs.FirstOrDefaultAsync(x => x.Id == id, ct);
        }

        public async Task<IReadOnlyList<ApprovalJob>> ListPendingAsync(int skip, int take, CancellationToken ct = default)
        {
            return await _db.ApprovalJobs.Where(j => j.DecidedAt == null) .OrderByDescending(j => j.RequestedAt).Skip(skip).Take(take).ToListAsync(ct);
        }
    }
}
