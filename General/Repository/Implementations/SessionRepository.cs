using Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace General.Repository.Implementations
{
    public class SessionRepository : ISessionRepository
    {
        public Task<Session> AddAsync(Session entity)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Session>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Session?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Session> UpdateAsync(Session entity)
        {
            throw new NotImplementedException();
        }
    }
}
