using Application.Dtos.Cart;
using General.Dto.Cart;

namespace Application.Services
{
    /// <summary>
    /// Defines the contract for managing shopping cart operations within the application layer.
    /// This interface is documented by AI.
    /// </summary>
    public interface ICartService
    {
        /// <summary>
        /// Retrieves the shopping cart associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart is being retrieved.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the user's <see cref="CartResponse"/> data.
        /// </returns>
        Task<CartResponse> GetAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Adds a new item to the user's shopping cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart will be updated.</param>
        /// <param name="dto">The data transfer object containing item details to be added to the cart.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CartResponse"/> after the item has been added.
        /// </returns>
        Task<CartResponse> AddItemAsync(int userId, AddCartItemRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Updates the quantity of an existing item in the user's shopping cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart is being updated.</param>
        /// <param name="dto">The data transfer object containing product and quantity information.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CartResponse"/> reflecting the new quantity.
        /// </returns>
        Task<CartResponse> UpdateQuantityAsync(int userId, UpdateCartItemRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Removes a specific item from the user's shopping cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart will be modified.</param>
        /// <param name="productId">The unique identifier of the product to remove from the cart.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CartResponse"/> after the item has been removed.
        /// </returns>
        Task<CartResponse> RemoveItemAsync(int userId, int productId, CancellationToken ct = default);

        /// <summary>
        /// Removes all items from the user's shopping cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart will be cleared.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains an empty <see cref="CartResponse"/> after the cart has been cleared.
        /// </returns>
        Task<CartResponse> ClearAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Applies a coupon code to the user's shopping cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose cart will be updated.</param>
        /// <param name="dto">The data transfer object containing coupon details.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CartResponse"/> reflecting the applied discount.
        /// </returns>
        Task<CartResponse> ApplyCouponAsync(int userId, ApplyCouponRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Removes the currently applied coupon from the user's shopping cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user whose coupon will be removed.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CartResponse"/> after the coupon has been removed.
        /// </returns>
        Task<CartResponse> RemoveCouponAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Performs the checkout process for the user's shopping cart.
        /// </summary>
        /// <param name="userId">The unique identifier of the user performing the checkout.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>A task representing the asynchronous operation.</returns>
        Task CheckoutAsync(int userId, CancellationToken ct = default);
    }
}
