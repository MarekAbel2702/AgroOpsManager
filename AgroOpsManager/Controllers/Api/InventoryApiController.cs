using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Web.Dtos.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Web.Controllers.Api;

[ApiController]
[Route("api/inventory")]
public class InventoryApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public InventoryApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("low-stock")]
    [ProducesResponseType(typeof(IEnumerable<InventoryItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<InventoryItemDto>>> GetLowStock()
    {
        var items = await _context.InventoryItems
            .OrderBy(x => x.Name)
            .ToListAsync();

        var dto = items
            .Where(x => x.IsLowStock())
            .Select(x => new InventoryItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Category = x.Category.ToString(),
                Unit = x.Unit,
                Quantity = x.Quantity,
                MinimumQuantity = x.MinimumQuantity,
                UnitPrice = x.UnitPrice,
                StockValue = x.Quantity * x.UnitPrice,
                IsLowStock = x.IsLowStock(),
                MissingToMinimum = x.MinimumQuantity > x.Quantity
                    ? x.MinimumQuantity - x.Quantity
                    : 0,
                ExpirationDate = x.ExpirationDate,
                SupplierName = x.SupplierName
            })
            .ToList();

        return Ok(dto);
    }
}