using Application.Services;
using Asp.Versioning;
using General.Dto.Inventory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    /// <summary>
    /// API controller responsible for managing inventory records,
    /// allowing administrators and employees to view paginated inventory data.
    /// This controller is documented by AI.
    /// </summary>
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IInventoryService _inventoryService;

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryController"/> class.
        /// </summary>
        /// <param name="inventoryService">The service responsible for inventory business logic and data access.</param>
        public InventoryController(IInventoryService inventoryService)
        {
            _inventoryService = inventoryService;
        }

        /// <summary>
        /// Retrieves a paginated list of inventory records along with the total count.
        /// </summary>
        /// <param name="skip">The number of records to skip before collecting results. Defaults to 0.</param>
        /// <param name="take">The number of records to take. Defaults to 50.</param>
        /// <param name="ct">An optional <see cref="CancellationToken"/> to observe while waiting for the task to complete.</param>
        /// <returns>
        /// <c>200 OK</c> with an object containing <c>Total</c> and <c>Items</c> (a list of <see cref="InventoryResponse"/>).
        /// </returns>
        /// <response code="200">Returns the list of inventory records with total count.</response>
        [Authorize(Roles = "ADMIN, EMPLOYEE")]
        [HttpGet]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public async Task<IActionResult> List(
            [FromQuery] int skip = 0,
            [FromQuery] int take = 50,
            CancellationToken ct = default)
        {
            var (items, total) = await _inventoryService.ListWithCountAsync(skip, take, ct);
            return Ok(new { Total = total, Items = items });
        }
    }
}
