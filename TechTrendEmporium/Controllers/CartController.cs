using Application.Dtos.Cart;
using Application.Services;
using Asp.Versioning;
using General.Dto.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    /// <summary>
    /// API controller for managing the user's shopping cart, including adding, updating, and removing items,
    /// applying coupons, and performing checkout.
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        /// <summary>
        /// Gets the ID of the currently authenticated user from their claims.
        /// </summary>
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Initializes a new instance of the <see cref="CartController"/> class.
        /// </summary>
        /// <param name="cartService">The cart service responsible for managing shopping cart operations.</param>
        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        /// <summary>
        /// Retrieves the current user's shopping cart.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The <see cref="CartResponse"/> containing the user's cart details.</returns>
        /// <response code="200">Returns the current user's shopping cart.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpGet]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> Get(CancellationToken ct)
        {
            return Ok(await _cartService.GetAsync(CurrentUserId, ct));
        }

        /// <summary>
        /// Adds an item to the user's shopping cart.
        /// </summary>
        /// <param name="dto">The item to add, including product ID and quantity.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The updated <see cref="CartResponse"/> after the item has been added.</returns>
        /// <response code="200">Returns the updated shopping cart.</response>
        /// <response code="400">If the request is invalid or stock is insufficient.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpPost("items")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> AddItem([FromBody] AddCartItemRequest dto, CancellationToken ct)
        {
            return Ok(await _cartService.AddItemAsync(CurrentUserId, dto, ct));
        }

        /// <summary>
        /// Updates the quantity of an existing item in the user's shopping cart.
        /// </summary>
        /// <param name="dto">The item to update, including product ID and new quantity.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The updated <see cref="CartResponse"/> after the quantity change.</returns>
        /// <response code="200">Returns the updated cart.</response>
        /// <response code="404">If the product is not found in the cart.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpPut("items")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> UpdateQuantity([FromBody] UpdateCartItemRequest dto, CancellationToken ct)
        {
            return Ok(await _cartService.UpdateQuantityAsync(CurrentUserId, dto, ct));
        }

        /// <summary>
        /// Removes a specific item from the user's shopping cart.
        /// </summary>
        /// <param name="productId">The unique identifier of the product to remove.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The updated <see cref="CartResponse"/> after the item has been removed.</returns>
        /// <response code="200">Returns the updated cart.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpDelete("items/{productId:int}")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> RemoveItem([FromRoute] int productId, CancellationToken ct)
        {
            return Ok(await _cartService.RemoveItemAsync(CurrentUserId, productId, ct));
        }

        /// <summary>
        /// Clears all items from the user's shopping cart.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The empty <see cref="CartResponse"/> after clearing.</returns>
        /// <response code="200">Returns the cleared cart.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpPost("clear")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> Clear(CancellationToken ct)
        {
            return Ok(await _cartService.ClearAsync(CurrentUserId, ct));
        }

        /// <summary>
        /// Applies a coupon code to the user's shopping cart.
        /// </summary>
        /// <param name="dto">The request containing the coupon code to apply.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The updated <see cref="CartResponse"/> after the coupon has been applied.</returns>
        /// <response code="200">Returns the updated cart with the applied coupon.</response>
        /// <response code="400">If the coupon is invalid or expired.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpPost("coupon")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> ApplyCoupon([FromBody] ApplyCouponRequest dto, CancellationToken ct)
        {
            return Ok(await _cartService.ApplyCouponAsync(CurrentUserId, dto, ct));
        }

        /// <summary>
        /// Removes any applied coupon from the user's shopping cart.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>The updated <see cref="CartResponse"/> without the coupon.</returns>
        /// <response code="200">Returns the cart with the coupon removed.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpDelete("coupon")]
        [ProducesResponseType(typeof(CartResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> RemoveCoupon(CancellationToken ct)
        {
            return Ok(await _cartService.RemoveCouponAsync(CurrentUserId, ct));
        }

        /// <summary>
        /// Performs the checkout process for the current user's shopping cart.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>No content if the checkout is completed successfully.</returns>
        /// <response code="204">The checkout was successful and the cart was cleared.</response>
        /// <response code="400">If there is an issue with the cart contents or inventory availability.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpPost("checkout")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Checkout(CancellationToken ct)
        {
            await _cartService.CheckoutAsync(CurrentUserId, ct);
            return NoContent();
        }
    }
}
