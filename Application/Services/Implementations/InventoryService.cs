using Application.Abstraction;
using Application.Abstractions;
using General.Dto.Inventory;
using General.Mappers;

namespace Application.Services.Implementations
{
    /// <summary>
    /// Provides business logic for managing product inventory, including retrieval and listing with related product data.
    /// This class is documented by AI.
    /// </summary>
    public class InventoryService : IInventoryService
    {
        private readonly IInventoryRepository _inventoryRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryService"/> class.
        /// </summary>
        /// <param name="inventoryRepository">Repository for inventory data persistence and retrieval.</param>
        /// <param name="productRepository">Repository for accessing related product information.</param>
        /// <param name="unitOfWork">Unit of Work to coordinate database transactions.</param>
        public InventoryService(
            IInventoryRepository inventoryRepository,
            IProductRepository productRepository,
            IUnitOfWork unitOfWork)
        {
            _inventoryRepository = inventoryRepository;
            _productRepository = productRepository;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Retrieves a paginated list of inventory records and maps them to response DTOs including product names.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect results. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A read-only list of <see cref="InventoryResponse"/> objects, each containing inventory details
        /// and the corresponding product title.
        /// </returns>
        public async Task<IReadOnlyList<InventoryResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default)
        {
            var inventories = await _inventoryRepository.ListAsync(skip, take, ct);

            var productIds = inventories.Select(i => i.ProductId).Distinct().ToList();
            var products = await _productRepository.ListByIdsAsync(ct, productIds);

            var nameById = products.ToDictionary(p => p.Id, p => p.Title);

            return InventoryMapper.ToResponseList(inventories, nameById);
        }

        /// <summary>
        /// Retrieves a paginated list of inventory records and the total count of all inventory items.
        /// Includes related product names for better context in responses.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect results. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A tuple containing:
        /// <list type="bullet">
        /// <item><description><c>Items</c>: a list of <see cref="InventoryResponse"/> objects with product titles included.</description></item>
        /// <item><description><c>Total</c>: the total count of inventory records in the system.</description></item>
        /// </list>
        /// </returns>
        public async Task<(IReadOnlyList<InventoryResponse> Items, int Total)> ListWithCountAsync(
            int skip = 0,
            int take = 50,
            CancellationToken ct = default)
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
