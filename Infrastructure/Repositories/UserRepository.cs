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
    public class UserRepository : EfRepository<User>, IUserRepository
    {
        private readonly AppDbContext _db;

        public UserRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }

        public Task<User?> GetAsync(Expression<Func<User, bool>> predicate, bool asTracking = false, CancellationToken ct = default)
        {
            return (asTracking ? _db.Users.AsTracking() : _db.Users.AsNoTracking()).FirstOrDefaultAsync(predicate, ct);
        }

        public Task<IReadOnlyList<User>> ListByIdsAsync(CancellationToken ct = default, List<int> ids = null)
        {
            return Task.FromResult((IReadOnlyList<User>)_db.Users.Where(c => ids.Contains(c.Id)).ToList());
        }
    }
}
