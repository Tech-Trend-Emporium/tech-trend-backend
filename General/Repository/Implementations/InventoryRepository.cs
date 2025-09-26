using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Repository.Implementations
{
    public class InventoryRepository : IInventoryRepository
    {
        public Task<Inventory> AddAsync(Inventory entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Inventory>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Inventory?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Inventory> UpdateAsync(Inventory entity)
        {
            throw new NotImplementedException();
        }
    }
}
