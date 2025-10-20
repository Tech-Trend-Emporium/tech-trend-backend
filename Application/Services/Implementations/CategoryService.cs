using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Domain.Validations;
using General.Dto.Category;
using General.Mappers;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides business logic for managing product categories,
    /// including creation, update, deletion, and listing operations.
    /// This class is documented by AI.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="CategoryService"/> class.
        /// </summary>
        /// <param name="categoryRepository">Repository for category persistence and queries.</param>
        /// <param name="unitOfWork">Unit of Work to coordinate database transactions.</param>
        public CategoryService(ICategoryRepository categoryRepository, IUnitOfWork unitOfWork)
        {
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Counts the total number of categories available.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The total count of category entities.</returns>
        public Task<int> CountAsync(CancellationToken ct = default)
        {
            return _categoryRepository.CountAsync(null, ct);
        }

        /// <summary>
        /// Creates a new category after validating uniqueness of the category name.
        /// </summary>
        /// <param name="dto">The data transfer object containing the category details to create.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The created <see cref="CategoryResponse"/> object.</returns>
        /// <exception cref="ConflictException">Thrown if a category with the same name already exists.</exception>
        public async Task<CategoryResponse> CreateAsync(CreateCategoryRequest dto, CancellationToken ct = default)
        {
            var normalized = dto.Name.Trim().ToUpperInvariant();
            var exists = await _categoryRepository.ExistsAsync(c => c.Name.ToUpper() == normalized, ct);
            if (exists)
                throw new ConflictException(CategoryValidator.CategoryNameExists(dto.Name));

            var entity = CategoryMapper.ToEntity(dto);

            _categoryRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return CategoryMapper.ToResponse(entity);
        }

        /// <summary>
        /// Deletes a category by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the category to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>true</c> if the category was deleted; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _categoryRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return false;

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Checks whether a category with the given name already exists.
        /// </summary>
        /// <param name="name">The category name to check.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>true</c> if a category with the specified name exists; otherwise, <c>false</c>.</returns>
        public Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        {
            var normalized = (name ?? string.Empty).Trim().ToUpperInvariant();
            return _categoryRepository.ExistsAsync(c => c.Name.ToUpper() == normalized, ct);
        }

        /// <summary>
        /// Retrieves a category by its unique identifier.
        /// </summary>
        /// <param name="id">The ID of the category to retrieve.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A <see cref="CategoryResponse"/> if the category is found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<CategoryResponse?> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _categoryRepository.GetByIdAsync(ct, id);
            if (entity == null) return null;

            return CategoryMapper.ToResponse(entity);
        }

        /// <summary>
        /// Retrieves a paginated list of categories.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A read-only list of <see cref="CategoryResponse"/> objects.</returns>
        public async Task<IReadOnlyList<CategoryResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _categoryRepository.ListAsync(skip, take);
            return CategoryMapper.ToResponseList(entities);
        }

        /// <summary>
        /// Retrieves a paginated list of categories along with the total count of available records.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A tuple containing the list of categories and the total number of categories.
        /// </returns>
        public async Task<(IReadOnlyList<CategoryResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var listTask = await _categoryRepository.ListAsync(skip, take, ct);
            var total = await _categoryRepository.CountAsync(null, ct);

            var items = CategoryMapper.ToResponseList(listTask);
            return (items, total);
        }

        /// <summary>
        /// Updates an existing category with new information.
        /// </summary>
        /// <param name="id">The ID of the category to update.</param>
        /// <param name="dto">The data transfer object containing updated category details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The updated <see cref="CategoryResponse"/> object.</returns>
        /// <exception cref="NotFoundException">Thrown when the category does not exist.</exception>
        /// <exception cref="ConflictException">Thrown when another category already uses the same name.</exception>
        public async Task<CategoryResponse> UpdateAsync(int id, UpdateCategoryRequest dto, CancellationToken ct = default)
        {
            var entity = await _categoryRepository.GetByIdAsync(ct, id);
            if (entity is null)
                throw new NotFoundException(CategoryValidator.CategoryNotFound(id));

            var normalized = dto.Name.Trim().ToUpperInvariant();
            var nameTaken = await _categoryRepository.ExistsAsync(c => c.Id != id && c.Name.ToUpper() == normalized, ct);
            if (nameTaken)
                throw new ConflictException(CategoryValidator.CategoryNameExists(dto.Name));

            CategoryMapper.ApplyUpdate(entity, dto);

            _categoryRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return CategoryMapper.ToResponse(entity);
        }
    }
}
