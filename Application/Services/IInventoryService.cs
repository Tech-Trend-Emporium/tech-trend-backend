using General.Dto.Inventory;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for managing inventory-related operations within the application layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IInventoryService
    {
        /// <summary>
        /// Retrieves a paginated list of inventory records.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a read-only list of <see cref="InventoryResponse"/> objects.
        /// </returns>
        Task<IReadOnlyList<InventoryResponse>> ListAsync(int skip = 0, int take = 50, CancellationToken ct = default);

        /// <summary>
        /// Retrieves a paginated list of inventory records along with the total count of available items.
        /// </summary>
        /// <param name="skip">The number of records to skip before starting to collect the result set. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains a tuple with the list of <see cref="InventoryResponse"/> objects and the total number of inventory records.
        /// </returns>
        Task<(IReadOnlyList<InventoryResponse> Items, int Total)> ListWithCountAsync(int skip = 0, int take = 50, CancellationToken ct = default);
    }
}
