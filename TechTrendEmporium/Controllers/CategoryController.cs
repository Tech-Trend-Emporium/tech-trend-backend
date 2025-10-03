using Application.Services;
using Asp.Versioning;
using General.Dto.Category;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _categoryService.GetByIdAsync(id, ct);

            return result is null ? NotFound() : Ok(result);
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE, SHOPPER")]
        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CategoryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var (items, total) = await _categoryService.ListWithCountAsync(skip, take, ct);

            return Ok(new { Total = total, Items = items });
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest dto, CancellationToken ct)
        {
            var created = await _categoryService.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest dto, CancellationToken ct)
        {
            var updated = await _categoryService.UpdateAsync(id, dto, ct);

            return Ok(updated);
        }

        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _categoryService.DeleteAsync(id, ct);

            return deleted ? NoContent() : NotFound();
        }
    }
}
