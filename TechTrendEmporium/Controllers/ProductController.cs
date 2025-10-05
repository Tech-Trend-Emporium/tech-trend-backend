using Application.Abstraction;
using Application.Services;
using Asp.Versioning;
using General.Dto.Product;
using General.Dto.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _productService.GetByIdAsync(id, ct);

            return result is null ? NotFound() : Ok(result);
        }

        [Authorize]
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, [FromQuery] string? category = null, CancellationToken ct = default)
        {
            var (items, total) = await _productService.ListWithCountAsync(skip, take, category, ct);

            return Ok(new { Total = total, Items = items });
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPost]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateProductRequest dto, CancellationToken ct)
        {
            var created = await _productService.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPatch("{id:int}")]
        [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateProductRequest dto, CancellationToken ct)
        {
            var updated = await _productService.UpdateAsync(id, dto, ct);

            return Ok(updated);
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _productService.DeleteAsync(id, ct);

            return deleted ? NoContent() : NotFound();
        }
    }
}
