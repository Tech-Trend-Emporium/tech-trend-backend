using Application.Abstraction;
using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Repository
{
    public class ApprovalJobRepository : IApprovalJobRepository
    {
        public Task<ApprovalJob> AddAsync(ApprovalJob entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<ApprovalJob>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<ApprovalJob?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ApprovalJob> UpdateAsync(ApprovalJob entity)
        {
            throw new NotImplementedException();
        }
    }
}
