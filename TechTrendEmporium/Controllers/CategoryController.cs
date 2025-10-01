using Application.Services;
using General.Dto.Category;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(int id, CancellationToken ct)
        {
            var result = await _categoryService.GetByIdAsync(id, ct);

            return result is null ? NotFound() : Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(typeof(IReadOnlyList<CategoryResponse>), StatusCodes.Status200OK)]
        public async Task<IActionResult> List([FromQuery] int skip = 0, [FromQuery] int take = 50, CancellationToken ct = default)
        {
            var (items, total) = await _categoryService.ListWithCountAsync(skip, take, ct);

            return Ok(new { Total = total, Items = items });
        }

        [HttpPost]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create([FromBody] CreateCategoryRequest dto, CancellationToken ct)
        {
            var created = await _categoryService.CreateAsync(dto, ct);

            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }

        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryRequest dto, CancellationToken ct)
        {
            var updated = await _categoryService.UpdateAsync(id, dto, ct);

            return Ok(updated);
        }

        [HttpDelete("{id:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(int id, CancellationToken ct)
        {
            var deleted = await _categoryService.DeleteAsync(id, ct);

            return deleted ? NoContent() : NotFound();
        }
    }
}
