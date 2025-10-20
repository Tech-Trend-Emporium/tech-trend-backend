using General.Dto.WishList;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for managing wish list operations within the application layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface IWishListService
    {
        /// <summary>
        /// Retrieves the wish list associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose wish list is being retrieved.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the user's <see cref="WishListResponse"/>.
        /// </returns>
        Task<WishListResponse> GetAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Adds a new item to the user's wish list.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose wish list will be updated.</param>
        /// <param name="dto">The data transfer object containing the details of the item to add to the wish list.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the updated <see cref="WishListResponse"/> after the item has been added.
        /// </returns>
        Task<WishListResponse> AddItemAsync(int userId, AddWishListItemRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Removes a specific item from the user's wish list.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose wish list will be modified.</param>
        /// <param name="productId">The unique identifier of the product to remove from the wish list.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains the updated <see cref="WishListResponse"/> after the item has been removed.
        /// </returns>
        Task<WishListResponse> RemoveItemAsync(int userId, int productId, CancellationToken ct = default);

        /// <summary>
        /// Clears all items from the user's wish list.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose wish list will be cleared.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.  
        /// The task result contains an empty <see cref="WishListResponse"/> after the wish list has been cleared.
        /// </returns>
        Task<WishListResponse> ClearAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Moves a specific item from the user's wish list to their shopping cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user performing the action.</param>
        /// <param name="productId">The unique identifier of the product to move to the cart.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task MoveItemToCartAsync(int userId, int productId, CancellationToken ct = default);
    }
}
