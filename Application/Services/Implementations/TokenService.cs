using Data.Entities;
using Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration config;

        public TokenService(IConfiguration config)
        {
            this.config = config;
        }

        public (string accessToken, DateTime expiresUtc) CreateAccessToken(User user, IEnumerable<Claim>? extraClaims = null)
        {
            var jwt = config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt["SigningKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.UtcNow.AddMinutes(double.Parse(jwt["AccessTokenMinutes"]!));

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Email, user.Email),
                new(ClaimTypes.Role, user.Role.ToString()), 
                new("ss", user.SecurityStamp),              
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (extraClaims != null) claims.AddRange(extraClaims);

            var token = new JwtSecurityToken(
                issuer: jwt["Issuer"],
                audience: jwt["Audience"],
                claims: claims,
                expires: expires,
                signingCredentials: creds
            );
            return (new JwtSecurityTokenHandler().WriteToken(token), expires);
        }

        public RefreshToken CreateRefreshToken(int userId, int sessionId)
        {
            var bytes = RandomNumberGenerator.GetBytes(64);
            var token = Convert.ToBase64String(bytes);
            var days = int.Parse(config["Jwt:RefreshTokenDays"]!);

            return new RefreshToken
            {
                Token = token,
                UserId = userId,
                SessionId = sessionId,
                ExpiresAtUtc = DateTime.UtcNow.AddDays(days)
            };
        }
    }
}
