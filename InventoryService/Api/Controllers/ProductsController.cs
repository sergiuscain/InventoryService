using InventoryService.Application.Products.Commands.CreateProduct;
using InventoryService.Application.Products.Commands.DeleteProduct;
using InventoryService.Application.Products.Commands.ReleaseStock;
using InventoryService.Application.Products.Commands.ReserveStock;
using InventoryService.Application.Products.Commands.UpdateProduct;
using InventoryService.Application.Products.Queries.GetProductById;
using InventoryService.Application.Products.Queries.GetProducts;
using InventoryService.Application.Products.Queries.GetProductsByIds;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InventoryService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator) => _mediator = mediator;

        /// <summary>Создать продукт. Только для администраторов.</summary>
        /// <response code="201">Продукт создан.</response>
        /// <response code="401">Требуется аутентификация.</response>
        /// <response code="403">Требуется роль Admin.</response>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command, CancellationToken ct)
        {
            var id = await _mediator.Send(command, ct);
            return CreatedAtAction(nameof(GetById), new { id }, id);
        }

        /// <summary>Получить продукт по идентификатору.</summary>
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
        {
            var product = await _mediator.Send(new GetProductByIdQuery(id), ct);
            return product is null ? NotFound() : Ok(product);
        }

        /// <summary>Список продуктов (пагинация).</summary>
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 20, CancellationToken ct = default)
        {
            var products = await _mediator.Send(new GetProductsQuery(page, pageSize), ct);
            return Ok(products);
        }

        /// <summary>Продукты по списку идентификаторов.</summary>
        [HttpGet("by-ids")]
        public async Task<IActionResult> GetByIds([FromQuery] string ids, CancellationToken ct)
        {
            var parsed = (ids ?? string.Empty)
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(s => Guid.TryParse(s, out var g) ? g : (Guid?)null)
                .Where(g => g.HasValue)
                .Select(g => g!.Value)
                .ToList();

            var products = await _mediator.Send(new GetProductsByIdsQuery(parsed), ct);
            return Ok(products);
        }

        /// <summary>Обновить продукт. Только для администраторов.</summary>
        /// <response code="204">Продукт обновлён.</response>
        /// <response code="401">Требуется аутентификация.</response>
        /// <response code="403">Требуется роль Admin.</response>
        /// <response code="404">Продукт не найден.</response>
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command, CancellationToken ct)
        {
            if (id != command.Id) return BadRequest("Id mismatch");
            var updated = await _mediator.Send(command, ct);
            return updated ? NoContent() : NotFound();
        }

        /// <summary>Удалить продукт. Только для администраторов.</summary>
        /// <response code="204">Продукт удалён.</response>
        /// <response code="401">Требуется аутентификация.</response>
        /// <response code="403">Требуется роль Admin.</response>
        /// <response code="404">Продукт не найден.</response>
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
        {
            var deleted = await _mediator.Send(new DeleteProductCommand(id), ct);
            return deleted ? NoContent() : NotFound();
        }

        /// <summary>Зарезервировать остатки. Внутренний эндпоинт (вызывается OrderService).</summary>
        [HttpPost("reserve")]
        public async Task<IActionResult> Reserve([FromBody] ReserveStockCommand command, CancellationToken ct)
        {
            var ok = await _mediator.Send(command, ct);
            return ok ? Ok() : Conflict(new { message = "Not enough stock or product not found." });
        }

        /// <summary>Снять резерв. Внутренний эндпоинт (вызывается OrderService).</summary>
        [HttpPost("release")]
        public async Task<IActionResult> Release([FromBody] ReleaseStockCommand command, CancellationToken ct)
        {
            await _mediator.Send(command, ct);
            return NoContent();
        }
    }
}