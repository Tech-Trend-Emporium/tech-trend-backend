using Data.Entities;

namespace Application.Abstraction
{
    /// <summary>
    /// Interface for managing <see cref="Cart"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface ICartRepository : IEfRepository<Cart>
    {
        /// <summary>
        /// Retrieves a <see cref="Cart"/> entity associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart is being retrieved.</param>
        /// <param name="includeGraph">
        /// A flag indicating whether to include related navigation properties (e.g., cart items and products) 
        /// in the query result. Defaults to <c>true</c>.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the user's <see cref="Cart"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<Cart?> GetByUserIdAsync(int userId, bool includeGraph = true, CancellationToken ct = default);

        /// <summary>
        /// Creates a new <see cref="Cart"/> entity for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the cart will be created.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the newly created <see cref="Cart"/> entity.
        /// </returns>
        Task<Cart> CreateForUserAsync(int userId, CancellationToken ct = default);
    }
}
