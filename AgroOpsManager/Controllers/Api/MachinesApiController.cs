using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Web.Dtos.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Web.Controllers.Api;

[ApiController]
[Route("api/machines")]
public class MachinesApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public MachinesApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("service-alerts")]
    [ProducesResponseType(typeof(IEnumerable<MachineDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<MachineDto>>> GetServiceAlerts()
    {
        var machines = await _context.Machines
            .OrderBy(x => x.Name)
            .ToListAsync();

        var dto = machines
            .Where(x => x.RequiresService())
            .Select(x =>
            {
                var hoursSinceLastService = x.CurrentWorkingHours - x.WorkingHoursAtLastService;
                var exceededBy = hoursSinceLastService - x.ServiceIntervalHours;

                return new MachineDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = x.Type.ToString(),
                    SerialNumber = x.SerialNumber,
                    ProductionYear = x.ProductionYear,
                    CurrentWorkingHours = x.CurrentWorkingHours,
                    WorkingHoursAtLastService = x.WorkingHoursAtLastService,
                    ServiceIntervalHours = x.ServiceIntervalHours,
                    HoursSinceLastService = hoursSinceLastService,
                    ExceededServiceHoursBy = exceededBy,
                    RequiresService = true,
                    Status = x.Status.ToString()
                };
            })
            .ToList();

        return Ok(dto);
    }
}