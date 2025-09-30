using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
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
    }
}
