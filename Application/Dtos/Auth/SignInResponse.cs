using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Dtos.Auth
{
    public class SignInResponse
    {
        public string AccessToken { get; set; } = null!;
        public DateTime AccessTokenExpiresAtUtc { get; set; }
        public string RefreshToken { get; set; } = null!;
        public DateTime RefreshTokenExpiresAtUtc { get; set; }
        public string Role { get; set; } = null!;
        public int SessionId { get; set; }
    }
}
