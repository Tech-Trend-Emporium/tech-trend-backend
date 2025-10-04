using Application.Abstraction;
using Application.Abstractions;
using General.Dto.Product;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using General.Mappers;
using Microsoft.EntityFrameworkCore.Migrations.Operations;

namespace Application.Services.Implementations
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ProductResponse> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _productRepository.GetByIdAsync(ct, id);
            if (entity == null) return null;

            var category = await _categoryRepository.GetByIdAsync(ct, entity.CategoryId);
            string categoryName = category?.Name ?? "Unknown";

            return ProductMapper.ToResponse(entity, categoryName);
        }
       
        public async Task<IReadOnlyList<ProductResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _productRepository.ListAsync(skip, take, ct);   
            
            List<int> categoryIds = entities.Select(e => e.CategoryId).Distinct().ToList();
            var categories = await _categoryRepository.ListByIdsAsync(ct, categoryIds);
            List<string> categoryNames = entities.Select(e => categories.FirstOrDefault(c => c.Id == e.CategoryId)?.Name ?? "Unknown").ToList();

            return ProductMapper.ToResponseList(entities, categoryNames);
        }
        
        public async Task<ProductResponse> CreateAsync(CreateProductRequest dto, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest dto, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _productRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return (false);

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public async Task<(IReadOnlyList<ProductResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var listTask = await _productRepository.ListAsync(skip, take, ct);
            var total = await _productRepository.CountAsync(null, ct);

            List<int> categoryIds = listTask.Select(e => e.CategoryId).Distinct().ToList();
            var categories = await _categoryRepository.ListByIdsAsync(ct, categoryIds);
            List<string> categoryNames = listTask.Select(e => categories.FirstOrDefault(c => c.Id == e.CategoryId)?.Name ?? "Unknown").ToList();

            var productResponses = ProductMapper.ToResponseList(listTask, categoryNames);
            
            return (productResponses, total);
        }

        public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        {
            var normalized = (name ?? string.Empty).Trim().ToUpperInvariant();

            return await _productRepository.ExistsAsync(p => p.Title.ToUpper() == normalized, ct);
        }

        public async Task<int> CountAsync(CancellationToken ct = default)
        {
            return await _productRepository.CountAsync(null, ct);
        }
    }
}
