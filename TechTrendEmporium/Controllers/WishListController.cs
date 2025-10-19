using Application.Services;
using Asp.Versioning;
using General.Dto.WishList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    /// <summary>
    /// API controller for managing user wish lists. 
    /// Allows shoppers to view, add, remove, and clear wish list items, 
    /// as well as move products from the wish list to the shopping cart.
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;

        /// <summary>
        /// Retrieves the ID of the currently authenticated user from the JWT claims.
        /// </summary>
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        /// <summary>
        /// Initializes a new instance of the <see cref="WishListController"/> class.
        /// </summary>
        /// <param name="wishListService">The service responsible for wish list operations.</param>
        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }

        /// <summary>
        /// Retrieves the current user's wish list.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with the <see cref="WishListResponse"/> representing the user's wish list.
        /// </returns>
        /// <response code="200">Returns the user's wish list.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpGet]
        [ProducesResponseType(typeof(WishListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<WishListResponse>> Get(CancellationToken ct)
        {
            return Ok(await _wishListService.GetAsync(CurrentUserId, ct));
        }

        /// <summary>
        /// Adds a product to the user's wish list.
        /// </summary>
        /// <param name="dto">The product to add to the wish list.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with the updated <see cref="WishListResponse"/>.
        /// </returns>
        /// <response code="200">Returns the updated wish list after adding the item.</response>
        /// <response code="400">If the request is invalid or the product ID is missing.</response>
        /// <response code="404">If the specified product does not exist.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpPost("items")]
        [ProducesResponseType(typeof(WishListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<WishListResponse>> Add([FromBody] AddWishListItemRequest dto, CancellationToken ct)
        {
            return Ok(await _wishListService.AddItemAsync(CurrentUserId, dto, ct));
        }

        /// <summary>
        /// Removes a product from the user's wish list.
        /// </summary>
        /// <param name="productId">The unique identifier of the product to remove.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with the updated <see cref="WishListResponse"/>.
        /// </returns>
        /// <response code="200">Returns the updated wish list after removing the item.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpDelete("items/{productId:int}")]
        [ProducesResponseType(typeof(WishListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<WishListResponse>> Remove([FromRoute] int productId, CancellationToken ct)
        {
            return Ok(await _wishListService.RemoveItemAsync(CurrentUserId, productId, ct));
        }

        /// <summary>
        /// Clears all items from the user's wish list.
        /// </summary>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with an empty <see cref="WishListResponse"/>.
        /// </returns>
        /// <response code="200">Returns the cleared wish list.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpPost("clear")]
        [ProducesResponseType(typeof(WishListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<WishListResponse>> Clear(CancellationToken ct)
        {
            return Ok(await _wishListService.ClearAsync(CurrentUserId, ct));
        }

        /// <summary>
        /// Moves a product from the user's wish list to their shopping cart.
        /// </summary>
        /// <param name="productId">The unique identifier of the product to move.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>204 No Content</c> if the operation was successful or the product was not in the wish list.
        /// </returns>
        /// <response code="204">Product successfully moved to the cart (or not found in the wish list).</response>
        /// <response code="404">If the specified product does not exist.</response>
        [Authorize(Roles = "SHOPPER")]
        [HttpPost("items/{productId:int}/move-to-cart")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> MoveToCart([FromRoute] int productId, CancellationToken ct)
        {
            await _wishListService.MoveItemToCartAsync(CurrentUserId, productId, ct);
            return NoContent();
        }
    }
}
