using General.Dto.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IProductService
    {
        Task<ProductResponse> CreateAsync(CreateProductRequest dto, CancellationToken ct = default);
        Task<ProductResponse?> GetByIdAsync(int id, CancellationToken ct = default);
        Task<IReadOnlyList<ProductResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);
        Task<(IReadOnlyList<ProductResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, string? category = null, CancellationToken ct = default);
        Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest dto, CancellationToken ct = default);
        Task<bool> DeleteAsync(int id, CancellationToken ct = default);
        Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default);
        Task<int> CountAsync(CancellationToken ct = default);
    }
}
