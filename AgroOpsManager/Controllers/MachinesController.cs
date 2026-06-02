using AgroOpsManager.Core.Entities;
using AgroOpsManager.Core.Enums;
using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Web.ViewModels.Machines;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Web.Controllers;

public class MachinesController : Controller
{
    private readonly ApplicationDbContext _context;

    public MachinesController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? status, string? type)
    {
        var query = _context.Machines.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x =>
                x.Name.Contains(searchTerm) ||
                x.SerialNumber.Contains(searchTerm));
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<MachineStatus>(status, out var parsedStatus))
        {
            query = query.Where(x => x.Status == parsedStatus);
        }

        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<MachineType>(type, out var parsedType))
        {
            query = query.Where(x => x.Type == parsedType);
        }

        var machines = await query
            .OrderBy(x => x.Name)
            .ToListAsync();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.Status = status;
        ViewBag.Type = type;

        return View(machines);
    }

    public async Task<IActionResult> Details(int id)
    {
        var machine = await _context.Machines
            .Include(x => x.FieldWorks)
            .ThenInclude(x => x.Field)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (machine is null)
        {
            return NotFound();
        }

        return View(machine);
    }

    public IActionResult Create()
    {
        var viewModel = new MachineFormViewModel
        {
            Status = MachineStatus.Available,
            Type = MachineType.Tractor,
            ProductionYear = DateTime.UtcNow.Year,
            ServiceIntervalHours = 250
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MachineFormViewModel viewModel)
    {
        ValidateWorkingHours(viewModel);

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var machine = new Machine
        {
            Name = viewModel.Name,
            Type = viewModel.Type,
            SerialNumber = viewModel.SerialNumber,
            ProductionYear = viewModel.ProductionYear,
            CurrentWorkingHours = viewModel.CurrentWorkingHours,
            ServiceIntervalHours = viewModel.ServiceIntervalHours,
            WorkingHoursAtLastService = viewModel.WorkingHoursAtLastService,
            Status = viewModel.Status,
            Notes = viewModel.Notes
        };

        _context.Machines.Add(machine);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Maszyna została dodana.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var machine = await _context.Machines.FirstOrDefaultAsync(x => x.Id == id);

        if (machine is null)
        {
            return NotFound();
        }

        var viewModel = new MachineFormViewModel
        {
            Id = machine.Id,
            Name = machine.Name,
            Type = machine.Type,
            SerialNumber = machine.SerialNumber,
            ProductionYear = machine.ProductionYear,
            CurrentWorkingHours = machine.CurrentWorkingHours,
            ServiceIntervalHours = machine.ServiceIntervalHours,
            WorkingHoursAtLastService = machine.WorkingHoursAtLastService,
            Status = machine.Status,
            Notes = machine.Notes
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MachineFormViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest();
        }

        ValidateWorkingHours(viewModel);

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var machine = await _context.Machines.FirstOrDefaultAsync(x => x.Id == id);

        if (machine is null)
        {
            return NotFound();
        }

        machine.Name = viewModel.Name;
        machine.Type = viewModel.Type;
        machine.SerialNumber = viewModel.SerialNumber;
        machine.ProductionYear = viewModel.ProductionYear;
        machine.CurrentWorkingHours = viewModel.CurrentWorkingHours;
        machine.ServiceIntervalHours = viewModel.ServiceIntervalHours;
        machine.WorkingHoursAtLastService = viewModel.WorkingHoursAtLastService;
        machine.Status = viewModel.Status;
        machine.Notes = viewModel.Notes;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Maszyna została zaktualizowana.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var machine = await _context.Machines.FirstOrDefaultAsync(x => x.Id == id);

        if (machine is null)
        {
            return NotFound();
        }

        return View(machine);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var machine = await _context.Machines.FirstOrDefaultAsync(x => x.Id == id);

        if (machine is null)
        {
            return NotFound();
        }

        machine.IsDeleted = true;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Maszyna została usunięta.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> ServiceAlerts()
    {
        var machines = await _context.Machines
            .OrderBy(x => x.Name)
            .ToListAsync();

        var machinesRequiringService = machines
            .Where(x => x.RequiresService())
            .ToList();

        return View(machinesRequiringService);
    }

    private void ValidateWorkingHours(MachineFormViewModel viewModel)
    {
        if (viewModel.WorkingHoursAtLastService > viewModel.CurrentWorkingHours)
        {
            ModelState.AddModelError(
                nameof(viewModel.WorkingHoursAtLastService),
                "Motogodziny przy ostatnim serwisie nie mogą być większe niż aktualne motogodziny.");
        }
    }
}