using Application.Abstraction;
using Data.Entities;
using Infrastructure.DbContexts;
using Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class InventoryRepository : EfRepository<Inventory>, IInventoryRepository
    {
        private readonly AppDbContext _db;

        public InventoryRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
