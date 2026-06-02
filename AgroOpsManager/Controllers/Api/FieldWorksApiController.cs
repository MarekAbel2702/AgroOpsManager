using AgroOpsManager.Core.Enums;
using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Web.Dtos.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Web.Controllers.Api;

[ApiController]
[Route("api/field-works")]
public class FieldWorksApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FieldWorksApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(IEnumerable<FieldWorkDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FieldWorkDto>>> GetUpcoming()
    {
        var fieldWorks = await _context.FieldWorks
            .Include(x => x.Field)
            .Include(x => x.Machine)
            .Include(x => x.ResourceUsages)
            .Where(x => x.Status == FieldWorkStatus.Planned || x.Status == FieldWorkStatus.InProgress)
            .OrderBy(x => x.PlannedDate)
            .Take(10)
            .ToListAsync();

        var dto = fieldWorks
            .Select(x => new FieldWorkDto
            {
                Id = x.Id,
                FieldId = x.FieldId,
                FieldName = x.Field.Name,
                MachineId = x.MachineId,
                MachineName = x.Machine?.Name,
                Type = x.Type.ToString(),
                PlannedDate = x.PlannedDate,
                StartedAtUtc = x.StartedAtUtc,
                CompletedAtUtc = x.CompletedAtUtc,
                Status = x.Status.ToString(),
                LaborCost = x.LaborCost,
                ResourcesCost = x.CalculateResourcesCost(),
                TotalCost = x.CalculateTotalCost(),
                OperatorName = x.OperatorName
            })
            .ToList();

        return Ok(dto);
    }
}