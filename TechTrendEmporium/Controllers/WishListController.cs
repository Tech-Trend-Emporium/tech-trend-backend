using Application.Abstraction;
using Application.Services;
using Asp.Versioning;
using General.Dto.Category;
using General.Dto.WishList;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class WishListController : ControllerBase
    {
        private readonly IWishListService _wishListService;
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public WishListController(IWishListService wishListService)
        {
            _wishListService = wishListService;
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpGet]
        [ProducesResponseType(typeof(WishListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<WishListResponse>> Get(CancellationToken ct)
        {
            return Ok(await _wishListService.GetAsync(CurrentUserId, ct));
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpPost("items")]
        [ProducesResponseType(typeof(WishListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<WishListResponse>> Add([FromBody] AddWishListItemRequest dto, CancellationToken ct)
        {
            return Ok(await _wishListService.AddItemAsync(CurrentUserId, dto, ct));
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpDelete("items/{productId:int}")]
        [ProducesResponseType(typeof(WishListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<WishListResponse>> Remove([FromRoute] int productId, CancellationToken ct)
        {
            return Ok(await _wishListService.RemoveItemAsync(CurrentUserId, productId, ct));
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpPost("clear")]
        [ProducesResponseType(typeof(WishListResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<WishListResponse>> Clear(CancellationToken ct)
        {
            return Ok(await _wishListService.ClearAsync(CurrentUserId, ct));
        }

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
