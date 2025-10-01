using Application.Dtos.Auth;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IAuthService
    {
        Task<SignUpResponse> SignUp(SignUpRequest dto, CancellationToken ct = default);
        Task<SignInResponse> SignIn(SignInRequest dto, CancellationToken ct = default);
        Task<SignInResponse> RefreshToken(RefreshTokenRequest dto, CancellationToken ct = default);
        Task SignOut(SignOutRequest dto, int currentUserId, CancellationToken ct = default);
    }
}
