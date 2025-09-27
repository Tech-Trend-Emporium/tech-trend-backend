using Application.Abstraction;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class CartRepository : ICartRepository
    {
        public Task<Cart> AddAsync(Cart entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Cart>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Cart?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Cart> UpdateAsync(Cart entity)
        {
            throw new NotImplementedException();
        }
    }
}
