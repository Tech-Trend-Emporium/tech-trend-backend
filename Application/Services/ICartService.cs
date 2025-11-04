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
        /// If the user does not yet have an active cart, a new one will be created automatically.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose cart is being retrieved.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the user's <see cref="CartResponse"/> data.
        /// </returns>
        Task<CartResponse> GetAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Adds a new item to the user's shopping cart or increases the quantity of an existing one.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose cart will be updated.
        /// </param>
        /// <param name="dto">
        /// The data transfer object containing item details such as product ID and quantity to add to the cart.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CartResponse"/> after the item has been added.
        /// </returns>
        Task<CartResponse> AddItemAsync(int userId, AddCartItemRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Updates the quantity of an existing item in the user's shopping cart.
        /// Removes the item if the new quantity is less than or equal to zero.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose cart is being updated.
        /// </param>
        /// <param name="dto">
        /// The data transfer object containing the product identifier and the desired quantity.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CartResponse"/> reflecting the new quantity.
        /// </returns>
        Task<CartResponse> UpdateQuantityAsync(int userId, UpdateCartItemRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Removes a specific product from the user's shopping cart.
        /// If the product is not found, the operation has no effect.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose cart will be modified.
        /// </param>
        /// <param name="productId">
        /// The unique identifier of the product to remove from the cart.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CartResponse"/> after the item has been removed.
        /// </returns>
        Task<CartResponse> RemoveItemAsync(int userId, int productId, CancellationToken ct = default);

        /// <summary>
        /// Removes all items from the user's shopping cart.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose cart will be cleared.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains an empty <see cref="CartResponse"/> after the cart has been cleared.
        /// </returns>
        Task<CartResponse> ClearAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Applies a valid coupon code to the user's shopping cart, adjusting totals accordingly.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose cart will be updated.
        /// </param>
        /// <param name="dto">
        /// The data transfer object containing the coupon code to apply.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CartResponse"/> reflecting the applied discount.
        /// </returns>
        Task<CartResponse> ApplyCouponAsync(int userId, ApplyCouponRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Removes any currently applied coupon from the user's shopping cart.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user whose coupon will be removed.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation.
        /// The task result contains the updated <see cref="CartResponse"/> after the coupon has been removed.
        /// </returns>
        Task<CartResponse> RemoveCouponAsync(int userId, CancellationToken ct = default);

        /// <summary>
        /// Finalizes the checkout process for the user's active cart.
        /// This operation validates stock availability, reserves inventory,
        /// records payment and shipping details, and converts the cart into a completed order.
        /// </summary>
        /// <param name="userId">
        /// The unique identifier of the user performing the checkout.
        /// </param>
        /// <param name="dto">
        /// The data transfer object containing checkout details such as shipping address,
        /// payment method, and optional transaction information.
        /// </param>
        /// <param name="ct">
        /// An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.
        /// </param>
        /// <returns>
        /// A task representing the asynchronous operation. 
        /// The task completes once the checkout process is successfully finalized.
        /// </returns>
        Task CheckoutAsync(int userId, CheckoutRequest dto, CancellationToken ct = default);

        /// <summary>
        /// Retrieves the current user's placed orders with pagination.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="skip">The number of records to skip. Defaults to 0.</param>
        /// <param name="take">The number of records to return. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/>.</param>
        /// <returns>
        /// A tuple with the list of <see cref="OrderResponse"/> items and the total count of placed orders.
        /// </returns>
        Task<(IReadOnlyList<OrderResponse> Items, int Total)> ListMyOrdersAsync(int userId, int skip = 0, int take = 50, CancellationToken ct = default);
    }
}
