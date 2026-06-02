using AgroOpsManager.Core.Enums;
using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Models;
using AgroOpsManager.Web.ViewModels.Dashboard;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace AgroOpsManager.Web.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly ApplicationDbContext _context;

    public HomeController(
        ILogger<HomeController> logger,
        ApplicationDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var activeFieldsCount = await _context.Fields
            .CountAsync(x => x.Status == FieldStatus.Active);

        var machines = await _context.Machines
            .OrderBy(x => x.Name)
            .ToListAsync();

        var machinesRequiringService = machines
            .Where(x => x.RequiresService())
            .ToList();

        var plannedFieldWorksCount = await _context.FieldWorks
            .CountAsync(x => x.Status == FieldWorkStatus.Planned);

        var completedFieldWorksCount = await _context.FieldWorks
            .CountAsync(x => x.Status == FieldWorkStatus.Completed);

        var fieldWorksForCost = await _context.FieldWorks
            .Include(x => x.ResourceUsages)
            .ToListAsync();

        var totalFieldWorksCost = fieldWorksForCost
            .Sum(x => x.CalculateTotalCost());

        var inventoryItems = await _context.InventoryItems
            .OrderBy(x => x.Name)
            .ToListAsync();

        var lowStockItems = inventoryItems
            .Where(x => x.IsLowStock())
            .ToList();

        var inventoryValue = inventoryItems
            .Sum(x => x.Quantity * x.UnitPrice);

        var upcomingFieldWorks = await _context.FieldWorks
            .Include(x => x.Field)
            .Include(x => x.Machine)
            .Where(x => x.Status == FieldWorkStatus.Planned || x.Status == FieldWorkStatus.InProgress)
            .OrderBy(x => x.PlannedDate)
            .Take(5)
            .ToListAsync();

        var recentlyCompletedFieldWorks = await _context.FieldWorks
            .Include(x => x.Field)
            .Include(x => x.Machine)
            .Include(x => x.ResourceUsages)
            .Where(x => x.Status == FieldWorkStatus.Completed)
            .OrderByDescending(x => x.CompletedAtUtc)
            .Take(5)
            .ToListAsync();

        var viewModel = new DashboardViewModel
        {
            ActiveFieldsCount = activeFieldsCount,
            MachinesCount = machines.Count,
            MachinesRequiringServiceCount = machinesRequiringService.Count,
            PlannedFieldWorksCount = plannedFieldWorksCount,
            CompletedFieldWorksCount = completedFieldWorksCount,
            LowStockItemsCount = lowStockItems.Count,
            TotalFieldWorksCost = totalFieldWorksCost,
            InventoryValue = inventoryValue,
            UpcomingFieldWorks = upcomingFieldWorks,
            RecentlyCompletedFieldWorks = recentlyCompletedFieldWorks,
            MachinesRequiringService = machinesRequiringService.Take(5).ToList(),
            LowStockItems = lowStockItems.Take(5).ToList()
        };

        return View(viewModel);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
        });
    }
}