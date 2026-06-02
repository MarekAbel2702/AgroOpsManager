using AgroOpsManager.Core.DTOs;
using AgroOpsManager.Core.Entities;
using AgroOpsManager.Core.Enums;
using AgroOpsManager.Core.Exceptions;
using AgroOpsManager.Core.Interfaces;
using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Web.ViewModels.FieldWorks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Web.Controllers;

public class FieldWorksController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly IFieldWorkCompletionService _fieldWorkCompletionService;

    public FieldWorksController(
        ApplicationDbContext context,
        IFieldWorkCompletionService fieldWorkCompletionService)
    {
        _context = context;
        _fieldWorkCompletionService = fieldWorkCompletionService;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? status, string? type)
    {
        var query = _context.FieldWorks
            .Include(x => x.Field)
            .Include(x => x.Machine)
            .Include(x => x.ResourceUsages)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x =>
                x.Field.Name.Contains(searchTerm) ||
                (x.Machine != null && x.Machine.Name.Contains(searchTerm)) ||
                (x.OperatorName != null && x.OperatorName.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<FieldWorkStatus>(status, out var parsedStatus))
        {
            query = query.Where(x => x.Status == parsedStatus);
        }

        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<FieldWorkType>(type, out var parsedType))
        {
            query = query.Where(x => x.Type == parsedType);
        }

        var fieldWorks = await query
            .OrderByDescending(x => x.PlannedDate)
            .ToListAsync();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.Status = status;
        ViewBag.Type = type;

        return View(fieldWorks);
    }

    public async Task<IActionResult> Details(int id)
    {
        var fieldWork = await _context.FieldWorks
            .Include(x => x.Field)
            .Include(x => x.Machine)
            .Include(x => x.ResourceUsages)
            .ThenInclude(x => x.InventoryItem)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (fieldWork is null)
        {
            return NotFound();
        }

        return View(fieldWork);
    }

    public async Task<IActionResult> Create()
    {
        var viewModel = new FieldWorkFormViewModel
        {
            PlannedDate = DateTime.UtcNow.Date,
            Status = FieldWorkStatus.Planned,
            Fields = await GetFieldsSelectListAsync(),
            Machines = await GetMachinesSelectListAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FieldWorkFormViewModel viewModel)
    {
        await ValidateFieldWorkAsync(viewModel);

        if (!ModelState.IsValid)
        {
            viewModel.Fields = await GetFieldsSelectListAsync();
            viewModel.Machines = await GetMachinesSelectListAsync();
            return View(viewModel);
        }

        var fieldWork = new FieldWork
        {
            FieldId = viewModel.FieldId,
            MachineId = viewModel.MachineId,
            Type = viewModel.Type,
            PlannedDate = viewModel.PlannedDate,
            Status = viewModel.Status,
            LaborCost = viewModel.LaborCost,
            OperatorName = viewModel.OperatorName,
            Notes = viewModel.Notes
        };

        if (fieldWork.Status == FieldWorkStatus.InProgress)
        {
            fieldWork.StartedAtUtc = DateTime.UtcNow;
        }

        if (fieldWork.Status == FieldWorkStatus.Completed)
        {
            fieldWork.CompletedAtUtc = DateTime.UtcNow;
        }

        _context.FieldWorks.Add(fieldWork);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Praca polowa została dodana.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var fieldWork = await _context.FieldWorks.FirstOrDefaultAsync(x => x.Id == id);

        if (fieldWork is null)
        {
            return NotFound();
        }

        var viewModel = new FieldWorkFormViewModel
        {
            Id = fieldWork.Id,
            FieldId = fieldWork.FieldId,
            MachineId = fieldWork.MachineId,
            Type = fieldWork.Type,
            PlannedDate = fieldWork.PlannedDate,
            Status = fieldWork.Status,
            LaborCost = fieldWork.LaborCost,
            OperatorName = fieldWork.OperatorName,
            Notes = fieldWork.Notes,
            Fields = await GetFieldsSelectListAsync(),
            Machines = await GetMachinesSelectListAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FieldWorkFormViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest();
        }

        await ValidateFieldWorkAsync(viewModel);

        if (!ModelState.IsValid)
        {
            viewModel.Fields = await GetFieldsSelectListAsync();
            viewModel.Machines = await GetMachinesSelectListAsync();
            return View(viewModel);
        }

        var fieldWork = await _context.FieldWorks.FirstOrDefaultAsync(x => x.Id == id);

        if (fieldWork is null)
        {
            return NotFound();
        }

        var previousStatus = fieldWork.Status;

        fieldWork.FieldId = viewModel.FieldId;
        fieldWork.MachineId = viewModel.MachineId;
        fieldWork.Type = viewModel.Type;
        fieldWork.PlannedDate = viewModel.PlannedDate;
        fieldWork.Status = viewModel.Status;
        fieldWork.LaborCost = viewModel.LaborCost;
        fieldWork.OperatorName = viewModel.OperatorName;
        fieldWork.Notes = viewModel.Notes;

        if (previousStatus != FieldWorkStatus.InProgress && fieldWork.Status == FieldWorkStatus.InProgress)
        {
            fieldWork.StartedAtUtc = DateTime.UtcNow;
        }

        if (previousStatus != FieldWorkStatus.Completed && fieldWork.Status == FieldWorkStatus.Completed)
        {
            fieldWork.CompletedAtUtc = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Praca polowa została zaktualizowana.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var fieldWork = await _context.FieldWorks
            .Include(x => x.Field)
            .Include(x => x.Machine)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (fieldWork is null)
        {
            return NotFound();
        }

        return View(fieldWork);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var fieldWork = await _context.FieldWorks.FirstOrDefaultAsync(x => x.Id == id);

        if (fieldWork is null)
        {
            return NotFound();
        }

        fieldWork.IsDeleted = true;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Praca polowa została usunięta.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Complete(int id)
    {
        var fieldWork = await _context.FieldWorks
            .Include(x => x.Field)
            .Include(x => x.ResourceUsages)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (fieldWork is null)
        {
            return NotFound();
        }

        if (fieldWork.Status == FieldWorkStatus.Completed)
        {
            TempData["ErrorMessage"] = "Ta praca jest już zakończona.";
            return RedirectToAction(nameof(Details), new { id });
        }

        if (fieldWork.Status == FieldWorkStatus.Cancelled)
        {
            TempData["ErrorMessage"] = "Nie można zakończyć anulowanej pracy.";
            return RedirectToAction(nameof(Details), new { id });
        }

        var viewModel = new CompleteFieldWorkViewModel
        {
            FieldWorkId = fieldWork.Id,
            FieldName = fieldWork.Field.Name,
            WorkType = fieldWork.Type.ToString(),
            PlannedDate = fieldWork.PlannedDate,
            LaborCost = fieldWork.LaborCost,
            CompletionNotes = fieldWork.Notes,
            Resources = await BuildResourceRowsAsync()
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Complete(int id, CompleteFieldWorkViewModel viewModel)
    {
        if (id != viewModel.FieldWorkId)
        {
            return BadRequest();
        }

        var fieldWork = await _context.FieldWorks
            .Include(x => x.Field)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (fieldWork is null)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            viewModel.FieldName = fieldWork.Field.Name;
            viewModel.WorkType = fieldWork.Type.ToString();
            viewModel.PlannedDate = fieldWork.PlannedDate;
            viewModel.LaborCost = fieldWork.LaborCost;
            viewModel.Resources = await RebuildResourceRowsAsync(viewModel.Resources);

            return View(viewModel);
        }

        var request = new CompleteFieldWorkRequest
        {
            FieldWorkId = id,
            CompletionNotes = viewModel.CompletionNotes,
            Resources = viewModel.Resources
                .Where(x => x.InventoryItemId.HasValue && x.QuantityUsed > 0)
                .Select(x => new ResourceUsageRequest
                {
                    InventoryItemId = x.InventoryItemId!.Value,
                    QuantityUsed = x.QuantityUsed
                })
                .ToList()
        };

        try
        {
            await _fieldWorkCompletionService.CompleteAsync(request);

            TempData["SuccessMessage"] = "Praca polowa została zakończona, a magazyn został zaktualizowany.";

            return RedirectToAction(nameof(Details), new { id });
        }
        catch (BusinessException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);

            viewModel.FieldName = fieldWork.Field.Name;
            viewModel.WorkType = fieldWork.Type.ToString();
            viewModel.PlannedDate = fieldWork.PlannedDate;
            viewModel.LaborCost = fieldWork.LaborCost;
            viewModel.Resources = await RebuildResourceRowsAsync(viewModel.Resources);

            return View(viewModel);
        }
    }

    private async Task<IEnumerable<SelectListItem>> GetFieldsSelectListAsync()
    {
        return await _context.Fields
            .Where(x => x.Status == FieldStatus.Active || x.Status == FieldStatus.Fallow)
            .OrderBy(x => x.Name)
            .Select(x => new SelectListItem
            {
                Value = x.Id.ToString(),
                Text = $"{x.Name} - {x.AreaInHectares:0.##} ha"
            })
            .ToListAsync();
    }

    private async Task<IEnumerable<SelectListItem>> GetMachinesSelectListAsync()
    {
        var machines = await _context.Machines
            .Where(x => x.Status == MachineStatus.Available || x.Status == MachineStatus.InUse)
            .OrderBy(x => x.Name)
            .ToListAsync();

        var selectList = new List<SelectListItem>
        {
            new SelectListItem
            {
                Value = "",
                Text = "Brak maszyny"
            }
        };

        selectList.AddRange(machines.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.Name} ({x.Type})"
        }));

        return selectList;
    }

    private async Task ValidateFieldWorkAsync(FieldWorkFormViewModel viewModel)
    {
        var fieldExists = await _context.Fields.AnyAsync(x => x.Id == viewModel.FieldId);

        if (!fieldExists)
        {
            ModelState.AddModelError(nameof(viewModel.FieldId), "Wybrane pole nie istnieje.");
        }

        if (viewModel.MachineId.HasValue)
        {
            var machine = await _context.Machines.FirstOrDefaultAsync(x => x.Id == viewModel.MachineId.Value);

            if (machine is null)
            {
                ModelState.AddModelError(nameof(viewModel.MachineId), "Wybrana maszyna nie istnieje.");
            }
            else if (machine.Status == MachineStatus.Broken || machine.Status == MachineStatus.Retired)
            {
                ModelState.AddModelError(nameof(viewModel.MachineId), "Nie można przypisać maszyny uszkodzonej lub wycofanej.");
            }
        }

        if (viewModel.Status == FieldWorkStatus.Completed && viewModel.PlannedDate.Date > DateTime.UtcNow.Date)
        {
            ModelState.AddModelError(nameof(viewModel.Status), "Nie można oznaczyć przyszłej pracy jako zakończonej.");
        }
    }

    private async Task<List<ResourceUsageInputViewModel>> BuildResourceRowsAsync()
    {
        var inventoryItems = await GetInventorySelectListAsync();

        return Enumerable.Range(0, 5)
            .Select(_ => new ResourceUsageInputViewModel
            {
                InventoryItems = inventoryItems
            })
            .ToList();
    }

    private async Task<List<ResourceUsageInputViewModel>> RebuildResourceRowsAsync(List<ResourceUsageInputViewModel> resources)
    {
        var inventoryItems = await GetInventorySelectListAsync();

        foreach (var resource in resources)
        {
            resource.InventoryItems = inventoryItems;
        }

        while (resources.Count < 5)
        {
            resources.Add(new ResourceUsageInputViewModel
            {
                InventoryItems = inventoryItems
            });
        }

        return resources;
    }

    private async Task<IEnumerable<SelectListItem>> GetInventorySelectListAsync()
    {
        var items = await _context.InventoryItems
            .OrderBy(x => x.Name)
            .ToListAsync();

        var selectList = new List<SelectListItem>
    {
        new SelectListItem
        {
            Value = "",
            Text = "Brak materiału"
        }
    };

        selectList.AddRange(items.Select(x => new SelectListItem
        {
            Value = x.Id.ToString(),
            Text = $"{x.Name} - dostępne: {x.Quantity:0.##} {x.Unit}, cena: {x.UnitPrice:0.00} zł/{x.Unit}"
        }));

        return selectList;
    }
}