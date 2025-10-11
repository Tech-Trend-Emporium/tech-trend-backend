using Application.Dtos.Cart;
using Application.Services;
using Asp.Versioning;
using General.Dto.Cart;
using General.Dto.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private int CurrentUserId => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpGet]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> Get(CancellationToken ct)
        {
            return Ok(await _cartService.GetAsync(CurrentUserId, ct));
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpPost("items")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> AddItem([FromBody] AddCartItemRequest dto, CancellationToken ct)
        {
            return Ok(await _cartService.AddItemAsync(CurrentUserId, dto, ct));
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpPut("items")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> UpdateQuantity([FromBody] UpdateCartItemRequest dto, CancellationToken ct)
        {
            return Ok(await _cartService.UpdateQuantityAsync(CurrentUserId, dto, ct));
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpDelete("items/{productId:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> RemoveItem([FromRoute] int productId, CancellationToken ct)
        {
            return Ok(await _cartService.RemoveItemAsync(CurrentUserId, productId, ct));
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpPost("clear")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> Clear(CancellationToken ct)
        {
            return Ok(await _cartService.ClearAsync(CurrentUserId, ct));
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpPost("coupon")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> ApplyCoupon([FromBody] ApplyCouponRequest dto, CancellationToken ct)
        {
            return Ok(await _cartService.ApplyCouponAsync(CurrentUserId, dto, ct));
        }

        [Authorize(Roles = "SHOPPER")]
        [HttpDelete("coupon")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<ActionResult<CartResponse>> RemoveCoupon(CancellationToken ct)
        {
            return Ok(await _cartService.RemoveCouponAsync(CurrentUserId, ct));
        }

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
