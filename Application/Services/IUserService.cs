using General.Dto.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IUserService
    {
        Task<UserResponse> CreateAsync(CreateUserRequest dto, CancellationToken ct = default);
        Task<UserResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<UserResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<(IReadOnlyList<UserResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<UserResponse> UpdateAsync(int id, UpdateUserRequest dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsByUsernameAsync(string username, CancellationToken ct = default);
        Task<bool> ExistsByEmailAsync(string email, CancellationToken ct = default);
        Task<int> CountAsync(CancellationToken ct = default);
    }
}
