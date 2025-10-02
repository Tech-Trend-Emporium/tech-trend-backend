using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class SessionRepository : EfRepository<Session>, ISessionRepository
    {
        private readonly AppDbContext _db;

        public SessionRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public async Task<List<Session>> GetActiveByUserAsync(int userId, CancellationToken ct = default)
        {
            return await _db.Sessions.Where(s => s.UserId == userId && s.IsActive).ToListAsync(ct);
        }
    }
}
