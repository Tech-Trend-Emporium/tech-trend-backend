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
            var entities = await _inventoryRepository.ListAsync(skip, take, ct);

            List<int> productIds = entities.Select(e => e.ProductId).Distinct().ToList();
            var products = await _productRepository.ListByIdsAsync(ct, productIds);
            List<string> productNames = entities.Select(e => products.FirstOrDefault(p => p.Id == e.ProductId)?.Title ?? "Unknown").ToList();

            return InventoryMapper.ToResponseList(entities, productNames);
        }

        public async Task<(IReadOnlyList<InventoryResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var listTask = await _inventoryRepository.ListAsync(skip, take, ct);
            var total = await _inventoryRepository.CountAsync(null, ct);

            List<int> productIds = listTask.Select(e => e.ProductId).Distinct().ToList();
            var products = await _productRepository.ListByIdsAsync(ct, productIds);
            List<string> productNames = listTask.Select(e => products.FirstOrDefault(p => p.Id == e.ProductId)?.Title ?? "Unknown").ToList();

            var inventoryResponses = InventoryMapper.ToResponseList(listTask, productNames);

            return (inventoryResponses, total);
        }
    }
}
