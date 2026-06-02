using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Web.Dtos.Api;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Web.Controllers.Api;

[ApiController]
[Route("api/fields")]
public class FieldsApiController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public FieldsApiController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FieldDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<FieldDto>>> GetAll()
    {
        var fields = await _context.Fields
            .OrderBy(x => x.Name)
            .Select(x => new FieldDto
            {
                Id = x.Id,
                Name = x.Name,
                AreaInHectares = x.AreaInHectares,
                Location = x.Location,
                SoilType = x.SoilType.ToString(),
                CurrentCrop = x.CurrentCrop.ToString(),
                Status = x.Status.ToString(),
                Notes = x.Notes
            })
            .ToListAsync();

        return Ok(fields);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FieldDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<FieldDto>> GetById(int id)
    {
        var field = await _context.Fields
            .Where(x => x.Id == id)
            .Select(x => new FieldDto
            {
                Id = x.Id,
                Name = x.Name,
                AreaInHectares = x.AreaInHectares,
                Location = x.Location,
                SoilType = x.SoilType.ToString(),
                CurrentCrop = x.CurrentCrop.ToString(),
                Status = x.Status.ToString(),
                Notes = x.Notes
            })
            .FirstOrDefaultAsync();

        if (field is null)
        {
            return NotFound();
        }

        return Ok(field);
    }
}