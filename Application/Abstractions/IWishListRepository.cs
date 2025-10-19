using Data.Entities;

namespace Application.Abstraction
{
    /// <summary>
    /// Interface for managing <see cref="WishList"/> entities within the data layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IWishListRepository : IEfRepository<WishList>
    {
        /// <summary>
        /// Retrieves a <see cref="WishList"/> entity associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose wish list is being retrieved.</param>
        /// <param name="includeGraph">
        /// A flag indicating whether to include related navigation properties (e.g., items and products) 
        /// in the query result. Defaults to <c>true</c>.
        /// </param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the user's <see cref="WishList"/> if found; otherwise, <c>null</c>.
        /// </returns>
        Task<WishList?> GetByUserIdAsync(int userId, bool includeGraph = true, CancellationToken ct = default);

        /// <summary>
        /// Creates a new <see cref="WishList"/> entity for a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user for whom the wish list will be created.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the newly created <see cref="WishList"/> entity.
        /// </returns>
        Task<WishList> CreateForUserAsync(int userId, CancellationToken ct = default);
    }
}
