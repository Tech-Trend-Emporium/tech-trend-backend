using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Repository.Implementations
{
    public class ProductRepository : IProductRepository
    {
        public Task<Product> AddAsync(Product entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Product>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Product?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Product> UpdateAsync(Product entity)
        {
            throw new NotImplementedException();
        }
    }
}
