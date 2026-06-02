using AgroOpsManager.Core.Enums;
using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Web.Dtos.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Web.Controllers.Api;

[ApiController]
[Route("api/dashboard")]
public class DashboardApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public DashboardApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("summary")]
    [ProducesResponseType(typeof(DashboardSummaryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardSummaryDto>> GetSummary()
    {
        var activeFieldsCount = await _context.Fields
            .CountAsync(x => x.Status == FieldStatus.Active);

        var machines = await _context.Machines.ToListAsync();

        var machinesRequiringServiceCount = machines
            .Count(x => x.RequiresService());

        var plannedFieldWorksCount = await _context.FieldWorks
            .CountAsync(x => x.Status == FieldWorkStatus.Planned);

        var completedFieldWorksCount = await _context.FieldWorks
            .CountAsync(x => x.Status == FieldWorkStatus.Completed);

        var inventoryItems = await _context.InventoryItems.ToListAsync();

        var lowStockItemsCount = inventoryItems
            .Count(x => x.IsLowStock());

        var inventoryValue = inventoryItems
            .Sum(x => x.Quantity * x.UnitPrice);

        var fieldWorks = await _context.FieldWorks
            .Include(x => x.ResourceUsages)
            .ToListAsync();

        var totalFieldWorksCost = fieldWorks
            .Sum(x => x.CalculateTotalCost());

        var dto = new DashboardSummaryDto
        {
            ActiveFieldsCount = activeFieldsCount,
            MachinesCount = machines.Count,
            MachinesRequiringServiceCount = machinesRequiringServiceCount,
            PlannedFieldWorksCount = plannedFieldWorksCount,
            CompletedFieldWorksCount = completedFieldWorksCount,
            LowStockItemsCount = lowStockItemsCount,
            TotalFieldWorksCost = totalFieldWorksCost,
            InventoryValue = inventoryValue
        };

        return Ok(dto);
    }
}