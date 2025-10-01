using Application.Abstraction;
using Application.Abstractions;
using Data.Entities;
using Domain.Entities;
using Infrastructure.DbContexts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class RefreshTokenRepository : EfRepository<RefreshToken>, IRefreshTokenRepository
    {
        private readonly AppDbContext _db;

        public RefreshTokenRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default)
        {
            return await _db.RefreshTokens.FirstOrDefaultAsync(r => r.Token == token, ct);
        }

        public async Task RevokeAllActiveByUserAsync(int userId, CancellationToken ct = default)
        {
            var q = _db.RefreshTokens.Where(r => r.UserId == userId && r.RevokedAtUtc == null && r.ExpiresAtUtc > DateTime.UtcNow);

            await q.ExecuteUpdateAsync(s => s.SetProperty(r => r.RevokedAtUtc, DateTime.UtcNow), ct);
        }

        public async Task RevokeByTokenAsync(string token, CancellationToken ct = default)
        {
            var q = _db.RefreshTokens.Where(r => r.Token == token && r.RevokedAtUtc == null && r.ExpiresAtUtc > DateTime.UtcNow);

            await q.ExecuteUpdateAsync(s => s.SetProperty(r => r.RevokedAtUtc, DateTime.UtcNow), ct);
        }
    }
}
