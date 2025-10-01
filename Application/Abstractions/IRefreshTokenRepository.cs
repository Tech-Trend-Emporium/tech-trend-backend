using Application.Abstraction;
using Data.Entities;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Abstractions
{
    public interface IRefreshTokenRepository : IEfRepository<RefreshToken>
    {
        Task<RefreshToken?> GetByTokenAsync(string token, CancellationToken ct = default);
        Task RevokeAllActiveByUserAsync(int userId, CancellationToken ct = default);
        Task RevokeByTokenAsync(string token, CancellationToken ct = default);
    }
}
