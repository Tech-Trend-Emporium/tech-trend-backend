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
    public class ProductRepository : EfRepository<Product>, IProductRepository
    {
        private readonly AppDbContext _db;

        public ProductRepository(AppDbContext db) : base(db)
        {
            _db = db;
        }
    }
}
