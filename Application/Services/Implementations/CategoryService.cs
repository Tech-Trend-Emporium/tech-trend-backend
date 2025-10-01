using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using General.Dto.Category;
using General.Mappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services.Implementations
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        public Task<int> CountAsync(CancellationToken ct = default)
        {
            return _categoryRepository.CountAsync(null, ct);
        }

        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest dto, CancellationToken ct = default)
        {
            var normalized = dto.Name.Trim().ToUpperInvariant();
            var exists = await _categoryRepository.ExistsAsync(c => c.Name.ToUpper() == normalized, ct);
            if (exists) throw new ConflictException($"Category with name '{dto.Name}' already exists.");

            var entity = CategoryMapper.ToEntity(dto);

            _categoryRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return CategoryMapper.ToResponse(entity);
        }

        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _categoryRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return false;

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        {
            var normalized = (name ?? string.Empty).Trim().ToUpperInvariant();

            return _categoryRepository.ExistsAsync(c => c.Name.ToUpper() == normalized, ct);
        }

        public async Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _categoryRepository.GetByIdAsync(ct, id);
            if (entity == null) return null;

            return CategoryMapper.ToResponse(entity);
        }

        public async Task<IReadOnlyList<CategoryResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _categoryRepository.ListAsync(skip, take);

            return CategoryMapper.ToResponseList(entities);
        }

        public async Task<(IReadOnlyList<CategoryResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var listTask = _categoryRepository.ListAsync(skip, take, ct);
            var countTask = _categoryRepository.CountAsync(null, ct);

            await Task.WhenAll(listTask, countTask);

            var items = CategoryMapper.ToResponseList(listTask.Result);
            var total = countTask.Result;

            return (items, total);
        }

        public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest dto, CancellationToken ct = default)
        {
            var entity = await _categoryRepository.GetByIdAsync(ct, id);
            if (entity is null) throw new NotFoundException($"The category with ID '{id}' was not found.");

            var normalized = dto.Name.Trim().ToUpperInvariant();
            var nameTaken = await _categoryRepository.ExistsAsync(c => c.Id != id && c.Name.ToUpper() == normalized, ct);
            if (nameTaken) throw new ConflictException($"Category with name '{dto.Name}' already exists.");

            CategoryMapper.ApplyUpdate(entity, dto);

            _categoryRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return CategoryMapper.ToResponse(entity);
        }
    }
}
