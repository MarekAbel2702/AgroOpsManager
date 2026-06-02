using AgroOpsManager.Core.Entities;
using AgroOpsManager.Core.Enums;
using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Web.ViewModels.Inventory;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Web.Controllers;

public class InventoryController : Controller
{
    private readonly ApplicationDbContext _context;

    public InventoryController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index(string? searchTerm, string? category, bool? lowStockOnly)
    {
        var query = _context.InventoryItems.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(x =>
                x.Name.Contains(searchTerm) ||
                (x.SupplierName != null && x.SupplierName.Contains(searchTerm)));
        }

        if (!string.IsNullOrWhiteSpace(category) && Enum.TryParse<InventoryCategory>(category, out var parsedCategory))
        {
            query = query.Where(x => x.Category == parsedCategory);
        }

        if (lowStockOnly == true)
        {
            query = query.Where(x => x.Quantity <= x.MinimumQuantity);
        }

        var items = await query
            .OrderBy(x => x.Category)
            .ThenBy(x => x.Name)
            .ToListAsync();

        ViewBag.SearchTerm = searchTerm;
        ViewBag.Category = category;
        ViewBag.LowStockOnly = lowStockOnly;

        return View(items);
    }

    public async Task<IActionResult> Details(int id)
    {
        var item = await _context.InventoryItems
            .Include(x => x.ResourceUsages)
            .ThenInclude(x => x.FieldWork)
            .ThenInclude(x => x.Field)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    public IActionResult Create()
    {
        var viewModel = new InventoryItemFormViewModel
        {
            Category = InventoryCategory.Fertilizer,
            Unit = "kg"
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InventoryItemFormViewModel viewModel)
    {
        ValidateInventoryItem(viewModel);

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var item = new InventoryItem
        {
            Name = viewModel.Name,
            Category = viewModel.Category,
            Unit = viewModel.Unit,
            Quantity = viewModel.Quantity,
            MinimumQuantity = viewModel.MinimumQuantity,
            UnitPrice = viewModel.UnitPrice,
            ExpirationDate = viewModel.ExpirationDate,
            SupplierName = viewModel.SupplierName,
            Notes = viewModel.Notes
        };

        _context.InventoryItems.Add(item);
        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Pozycja magazynowa została dodana.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);

        if (item is null)
        {
            return NotFound();
        }

        var viewModel = new InventoryItemFormViewModel
        {
            Id = item.Id,
            Name = item.Name,
            Category = item.Category,
            Unit = item.Unit,
            Quantity = item.Quantity,
            MinimumQuantity = item.MinimumQuantity,
            UnitPrice = item.UnitPrice,
            ExpirationDate = item.ExpirationDate,
            SupplierName = item.SupplierName,
            Notes = item.Notes
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, InventoryItemFormViewModel viewModel)
    {
        if (id != viewModel.Id)
        {
            return BadRequest();
        }

        ValidateInventoryItem(viewModel);

        if (!ModelState.IsValid)
        {
            return View(viewModel);
        }

        var item = await _context.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);

        if (item is null)
        {
            return NotFound();
        }

        item.Name = viewModel.Name;
        item.Category = viewModel.Category;
        item.Unit = viewModel.Unit;
        item.Quantity = viewModel.Quantity;
        item.MinimumQuantity = viewModel.MinimumQuantity;
        item.UnitPrice = viewModel.UnitPrice;
        item.ExpirationDate = viewModel.ExpirationDate;
        item.SupplierName = viewModel.SupplierName;
        item.Notes = viewModel.Notes;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Pozycja magazynowa została zaktualizowana.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);

        if (item is null)
        {
            return NotFound();
        }

        return View(item);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var item = await _context.InventoryItems.FirstOrDefaultAsync(x => x.Id == id);

        if (item is null)
        {
            return NotFound();
        }

        item.IsDeleted = true;

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = "Pozycja magazynowa została usunięta.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> LowStock()
    {
        var items = await _context.InventoryItems
            .Where(x => x.Quantity <= x.MinimumQuantity)
            .OrderBy(x => x.Name)
            .ToListAsync();

        return View(items);
    }

    private void ValidateInventoryItem(InventoryItemFormViewModel viewModel)
    {
        if (viewModel.ExpirationDate.HasValue && viewModel.ExpirationDate.Value.Date < DateTime.UtcNow.Date)
        {
            ModelState.AddModelError(
                nameof(viewModel.ExpirationDate),
                "Data ważności nie może być z przeszłości.");
        }

        if (viewModel.MinimumQuantity > viewModel.Quantity && viewModel.Quantity == 0)
        {
            return;
        }
    }
}