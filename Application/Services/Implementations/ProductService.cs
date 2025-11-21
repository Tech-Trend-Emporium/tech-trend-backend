using Application.Abstraction;
using Application.Abstractions;
using Application.Exceptions;
using Data.Entities;
using Domain.Validations;
using General.Dto.Product;
using General.Mappers;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides business logic for managing products, including creation, update, deletion,
    /// retrieval, and listing operations. Coordinates with category and inventory repositories
    /// to ensure data consistency.
    /// This class is documented by AI.
    /// </summary>
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductService"/> class.
        /// </summary>
        /// <param name="productRepository">Repository for product data persistence and retrieval.</param>
        /// <param name="categoryRepository">Repository for category validation and lookups.</param>
        /// <param name="unitOfWork">Unit of Work to manage transactions and save changes.</param>
        public ProductService(
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            IUnitOfWork unitOfWork)
        {
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves a specific product by its unique identifier, including its category name.
        /// </summary>
        /// <param name="id">The unique identifier of the product.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// The <see cref="ProductResponse"/> if found; otherwise, <c>null</c>.
        /// </returns>
        public async Task<ProductResponse> GetByIdAsync(int id, CancellationToken ct = default)
        {
            var entity = await _productRepository.GetByIdAsync(ct, id);
            if (entity == null) return null;

            var category = await _categoryRepository.GetByIdAsync(ct, entity.CategoryId);

            return ProductMapper.ToResponse(entity, category.Name);
        }

        public async Task<ProductResponse> GetByNameAsync(string name, CancellationToken ct = default)
        {
            var entity = await _productRepository.GetAsync(p => p.Title.Trim().ToUpper().Contains(name.Trim().ToUpper()), asTracking: true, ct: ct);
            if (entity == null) return null;

            var category = await _categoryRepository.GetByIdAsync(ct, entity.CategoryId);

            return ProductMapper.ToResponse(entity, category.Name);
        }

        /// <summary>
        /// Retrieves a paginated list of products, including their associated category names.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A read-only list of <see cref="ProductResponse"/> objects.</returns>
        public async Task<IReadOnlyList<ProductResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var entities = await _productRepository.ListAsync(skip, take, ct);

            var categoryIds = entities.Select(e => e.CategoryId).Distinct().ToList();
            var categories = await _categoryRepository.ListByIdsAsync(ct, categoryIds);

            var nameById = categories.ToDictionary(c => c.Id, c => c.Name);
            return ProductMapper.ToResponseList(entities, nameById);
        }

        /// <summary>
        /// Creates a new product with an optional inventory configuration.
        /// Ensures the category exists and validates unique product title.
        /// </summary>
        /// <param name="dto">The product creation request data.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The created <see cref="ProductResponse"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="ConflictException">Thrown when a product with the same title already exists.</exception>
        /// <exception cref="NotFoundException">Thrown when the specified category does not exist.</exception>
        /// <exception cref="BadRequestException">Thrown when the inventory configuration is invalid.</exception>
        public async Task<ProductResponse> CreateAsync(CreateProductRequest dto, CancellationToken ct = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var productName = dto.Title.Trim().ToUpper();
            var product = await _productRepository.GetAsync(p => p.Title.Trim().ToUpper() == productName, asTracking: true, ct: ct);
            if (product is not null)
                throw new ConflictException(ProductValidator.ProductAlreadyExists(productName));

            var categoryName = dto.Category.Trim().ToUpper();
            var category = await _categoryRepository.GetAsync(c => c.Name.Trim().ToUpper() == categoryName, asTracking: true, ct: ct);
            if (category is null)
                throw new NotFoundException(CategoryValidator.CategoryNotFound(categoryName));

            var entity = ProductMapper.ToEntity(dto, category.Id);

            if (dto.Inventory is not null)
            {
                if (dto.Inventory.Available > dto.Inventory.Total)
                    throw new BadRequestException(InventoryValidator.AvailableGreaterThanTotalErrorMessage);

                entity.Inventory = InventoryMapper.ToEntity(dto.Inventory);
            }

            _productRepository.Add(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            return ProductMapper.ToResponse(entity, category.Name);
        }

        /// <summary>
        /// Updates an existing product and optionally modifies its inventory or category.
        /// </summary>
        /// <param name="id">The unique identifier of the product to update.</param>
        /// <param name="dto">The product update request data.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The updated <see cref="ProductResponse"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dto"/> is null.</exception>
        /// <exception cref="NotFoundException">Thrown when the product or its category cannot be found.</exception>
        /// <exception cref="BadRequestException">Thrown when inventory values are inconsistent.</exception>
        public async Task<ProductResponse> UpdateAsync(int id, UpdateProductRequest dto, CancellationToken ct = default)
        {
            if (dto is null)
                throw new ArgumentNullException(nameof(dto));

            var entity = await _productRepository.GetByIdAsync(ct, id);
            if (entity is null)
                throw new NotFoundException(ProductValidator.ProductNotFound(id));

            int? categoryId = null;
            string? categoryNameForResponse = null;

            if (!string.IsNullOrWhiteSpace(dto.Category))
            {
                var normalizedCategory = dto.Category.Trim().ToUpperInvariant();
                var category = await _categoryRepository.GetAsync(c => c.Name.Trim().ToUpper() == normalizedCategory, asTracking: true, ct: ct);

                if (category is null)
                    throw new NotFoundException(CategoryValidator.CategoryNotFound(dto.Category));

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

                    if (newAvailable > newTotal)
                        throw new BadRequestException(InventoryValidator.AvailableGreaterThanTotalErrorMessage);

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

                    if (newAvailable > newTotal)
                        throw new BadRequestException(InventoryValidator.AvailableGreaterThanTotalErrorMessage);

                    if (invDto.Total.HasValue) entity.Inventory.Total = invDto.Total.Value;
                    if (invDto.Available.HasValue) entity.Inventory.Available = invDto.Available.Value;
                }
            }

            entity.UpdatedAt = DateTime.UtcNow;

            _productRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync(ct);

            if (categoryNameForResponse is null)
                categoryNameForResponse = string.Empty;

            return ProductMapper.ToResponse(entity, categoryNameForResponse);
        }

        /// <summary>
        /// Deletes a product by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the product to delete.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>true</c> if the product was deleted; otherwise, <c>false</c>.</returns>
        public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        {
            var deleted = await _productRepository.DeleteByIdAsync(ct, id);
            if (!deleted) return false;

            await _unitOfWork.SaveChangesAsync(ct);
            return true;
        }

        /// <summary>
        /// Retrieves a paginated list of products, optionally filtered by category name,
        /// along with the total count of matching records.
        /// </summary>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="category">Optional category name to filter results.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>Items</c>: the paginated list of products.</description></item>
        /// <item><description><c>Total</c>: the total count of matching products.</description></item>
        /// </list>
        /// </returns>
        public async Task<(IReadOnlyList<ProductResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, string? category = null, CancellationToken ct = default)
        {
            int? categoryId = null;

            if (!string.IsNullOrWhiteSpace(category))
            {
                var normalized = category.Trim().ToUpperInvariant();
                var categoryEntity = await _categoryRepository.GetAsync(c => c.Name.Trim().ToUpper() == normalized, asTracking: true, ct: ct);

                if (categoryEntity != null)
                    categoryId = categoryEntity.Id;
            }

            var entities = await _productRepository.ListAsync(skip, take, categoryId, ct);
            var total = await _productRepository.CountAsync(categoryId, ct);

            var categoryIds = entities.Select(e => e.CategoryId).Distinct().ToList();
            var categories = await _categoryRepository.ListByIdsAsync(ct, categoryIds);
            var nameById = categories.ToDictionary(c => c.Id, c => c.Name);

            var items = ProductMapper.ToResponseList(entities, nameById);
            return (items, total);
        }

        /// <summary>
        /// Checks whether a product with the specified name exists.
        /// </summary>
        /// <param name="name">The product name to check.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns><c>true</c> if a product with the given name exists; otherwise, <c>false</c>.</returns>
        public async Task<bool> ExistsByNameAsync(string name, CancellationToken ct = default)
        {
            var normalized = (name ?? string.Empty).Trim().ToUpperInvariant();
            return await _productRepository.ExistsAsync(p => p.Title.ToUpper() == normalized, ct);
        }

        /// <summary>
        /// Returns the total number of products available in the system.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The total number of products.</returns>
        public async Task<int> CountAsync(CancellationToken ct = default)
        {
            return await _productRepository.CountAsync(null, ct);
        }
    }
}
