using Application.Abstraction;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class WishListRepository : IWishListRepository
    {
        public Task<WishList> AddAsync(WishList entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<WishList>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<WishList?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<WishList> UpdateAsync(WishList entity)
        {
            throw new NotImplementedException();
        }
    }
}
