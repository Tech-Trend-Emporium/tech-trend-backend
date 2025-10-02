using General.Dto.Category;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface ICategoryService
    {
        Task<CategoryResponse> CreateAsync(CreateCategoryRequest dto, CancellationToken ct = default);
        Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<CategoryResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<(IReadOnlyList<CategoryResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
        Task<int> CountAsync(CancellationToken ct = default);
    }
}
