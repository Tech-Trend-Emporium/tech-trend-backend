using Data.Entities;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ITokenService
    {
        (string accessToken, DateTime expiresUtc) CreateAccessToken(User user, IEnumerable<Claim>? extraClaims = null);
        RefreshToken CreateRefreshToken(int userId, int sessionId);
    }
}
