using Application.Abstraction;
using Application.Abstractions;
using General.Dto.Inventory;
using General.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        public InventoryService(IInventoryRepository inventoryRepository, IProductRepository productRepository, IUnitOfWork unitOfWork)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<IReadOnlyList<InventoryResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var inventories = await _inventoryRepository.ListAsync(skip, take, ct);

            var productIds = inventories.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepository.ListByIdsAsync(ct, productIds);

            var nameById = products.ToDictionary(p => p.Id, p => p.Title);

            return InventoryMapper.ToResponseList(inventories, nameById);
        }

        public async Task<(IReadOnlyList<InventoryResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var inventories = await _inventoryRepository.ListAsync(skip, take, ct);
            var total = await _inventoryRepository.CountAsync(null, ct);

            var productIds = inventories.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepository.ListByIdsAsync(ct, productIds);

            var nameById = products.ToDictionary(p => p.Id, p => p.Title);

            var items = InventoryMapper.ToResponseList(inventories, nameById);
            return (items, total);
        }
    }
}
