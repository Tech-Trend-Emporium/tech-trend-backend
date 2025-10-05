using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using Domain.Validations;
using General.Dto.Product;
using General.Mappers;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations.Operations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var categoryName = dto.Category.Trim().ToUpper();
            var category = await _categoryRepository.GetAsync(c => c.Name.Trim().ToUpper() == categoryName, asTracking: true, ct: ct);
            if (category is null) throw new NotFoundException(CategoryValidator.CategoryNotFound(categoryName));

            var entity = ProductMapper.ToEntity(dto, category.Id);

            if (dto.Inventory is not null)
            {
                if (dto.Inventory.Available > dto.Inventory.Total) throw new BadRequestException(InventoryValidator.AvailableGreaterThanTotalErrorMessage);

                entity.Inventory = InventoryMapper.ToEntity(dto.Inventory);
            }

            _productRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return ProductMapper.ToResponse(entity, category.Name);
        }

        public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest dto, CancellationToken ct = default)
        {
            if (dto is null) throw new ArgumentNullException(nameof(dto));

            var entity = await _productRepository.GetByIdAsync(ct, id);
            if (entity is null) throw new NotFoundException(ProductValidator.ProductNotFound(id));

            int? categoryId = null;
            string? categoryNameForResponse = null;
            if (!string.IsNullOrWhiteSpace(dto.Category))
            {
                var normalizedCategory = dto.Category.Trim().ToUpperInvariant();
                var category = await _categoryRepository.GetAsync(c => c.Name.Trim().ToUpper() == normalizedCategory, asTracking: true, ct: ct);

                if (category is null) throw new NotFoundException(CategoryValidator.CategoryNotFound(dto.Category));
                categoryId = category.Id;
                categoryNameForResponse = category.Name;
            }

            ProductMapper.ApplyUpdate(entity, dto, categoryId);

            if (dto.Inventory is not null)
            {
                var invDto = dto.Inventory;

                if (entity.Inventory is null)
                {
                    var newTotal = invDto.Total ?? 0;
                    var newAvailable = invDto.Available ?? 0;

                    if (newAvailable > newTotal) throw new BadRequestException(InventoryValidator.AvailableGreaterThanTotalErrorMessage);

                    entity.Inventory = new Inventory
                    {
                        Total = newTotal,
                        Available = newAvailable
                    };
                }
                else
                {
                    var currentTotal = entity.Inventory.Total;
                    var currentAvailable = entity.Inventory.Available;

                    var newTotal = invDto.Total ?? currentTotal;
                    var newAvailable = invDto.Available ?? currentAvailable;

                    if (newAvailable > newTotal) throw new BadRequestException(InventoryValidator.AvailableGreaterThanTotalErrorMessage);

                    if (invDto.Total.HasValue) entity.Inventory.Total = invDto.Total.Value;
                    if (invDto.Available.HasValue) entity.Inventory.Available = invDto.Available.Value;
                }
            }

            entity.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            if (categoryNameForResponse is null)
            {
                categoryNameForResponse = string.Empty;
            }

            return ProductMapper.ToResponse(entity, categoryNameForResponse);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _productRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return (false);

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public async Task<(IReadOnlyList<ProductResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, string? category = null, CancellationToken ct = default)
        {
            int? categoryId = null;
            if (!string.IsNullOrWhiteSpace(category))
            {
                var normalizedCategory = category.Trim().ToUpperInvariant();
                var categoryEntity = await _categoryRepository.GetAsync(c => c.Name.Trim().ToUpper() == normalizedCategory, asTracking: true, ct: ct);
                if (categoryEntity != null) categoryId = categoryEntity.Id;
            }

            var listTask = await _productRepository.ListAsync(skip, take, categoryId, ct);
            var total = await _productRepository.CountAsync(categoryId, ct);

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
