using AgroOpsManager.Core.Enums;
using AgroOpsManager.Infrastructure.Data;
using AgroOpsManager.Web.ViewModels.Reports;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AgroOpsManager.Web.Controllers;

public class ReportsController : Controller
{
    private readonly ApplicationDbContext _context;

    public ReportsController(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var fieldWorks = await _context.FieldWorks
            .Include(x => x.Field)
            .Include(x => x.ResourceUsages)
            .OrderByDescending(x => x.PlannedDate)
            .ToListAsync();

        var inventoryItems = await _context.InventoryItems
            .OrderBy(x => x.Name)
            .ToListAsync();

        var totalLaborCost = fieldWorks.Sum(x => x.LaborCost);
        var totalResourcesCost = fieldWorks.Sum(x => x.CalculateResourcesCost());
        var totalFieldWorksCost = fieldWorks.Sum(x => x.CalculateTotalCost());
        var inventoryValue = inventoryItems.Sum(x => x.Quantity * x.UnitPrice);

        var costsByWorkType = fieldWorks
            .GroupBy(x => x.Type)
            .Select(group =>
            {
                var laborCost = group.Sum(x => x.LaborCost);
                var resourcesCost = group.Sum(x => x.CalculateResourcesCost());
                var totalCost = laborCost + resourcesCost;

                return new CostByTypeReportItem
                {
                    WorkType = group.Key.ToString(),
                    Count = group.Count(),
                    LaborCost = laborCost,
                    ResourcesCost = resourcesCost,
                    TotalCost = totalCost,
                    Percentage = CalculatePercentage(totalCost, totalFieldWorksCost)
                };
            })
            .OrderByDescending(x => x.TotalCost)
            .ToList();

        var costsByField = fieldWorks
            .GroupBy(x => new
            {
                x.FieldId,
                x.Field.Name,
                x.Field.AreaInHectares
            })
            .Select(group =>
            {
                var totalCost = group.Sum(x => x.CalculateTotalCost());
                var area = group.Key.AreaInHectares;

                return new CostByFieldReportItem
                {
                    FieldId = group.Key.FieldId,
                    FieldName = group.Key.Name,
                    AreaInHectares = area,
                    Count = group.Count(),
                    TotalCost = totalCost,
                    CostPerHectare = area > 0 ? totalCost / area : 0,
                    Percentage = CalculatePercentage(totalCost, totalFieldWorksCost)
                };
            })
            .OrderByDescending(x => x.TotalCost)
            .ToList();

        var inventoryValueByCategory = inventoryItems
            .GroupBy(x => x.Category)
            .Select(group =>
            {
                var totalValue = group.Sum(x => x.Quantity * x.UnitPrice);

                return new InventoryValueByCategoryReportItem
                {
                    Category = group.Key.ToString(),
                    Count = group.Count(),
                    TotalValue = totalValue,
                    Percentage = CalculatePercentage(totalValue, inventoryValue)
                };
            })
            .OrderByDescending(x => x.TotalValue)
            .ToList();

        var fieldWorksByStatus = fieldWorks
            .GroupBy(x => x.Status)
            .Select(group => new FieldWorkStatusReportItem
            {
                Status = group.Key.ToString(),
                Count = group.Count(),
                Percentage = CalculatePercentage(group.Count(), fieldWorks.Count)
            })
            .OrderByDescending(x => x.Count)
            .ToList();

        var mostExpensiveFieldWorks = fieldWorks
            .Select(x => new TopFieldWorkCostReportItem
            {
                FieldWorkId = x.Id,
                FieldName = x.Field.Name,
                WorkType = x.Type.ToString(),
                PlannedDate = x.PlannedDate,
                LaborCost = x.LaborCost,
                ResourcesCost = x.CalculateResourcesCost(),
                TotalCost = x.CalculateTotalCost(),
                Status = x.Status.ToString()
            })
            .OrderByDescending(x => x.TotalCost)
            .Take(10)
            .ToList();

        var viewModel = new ReportsViewModel
        {
            TotalFieldWorksCost = totalFieldWorksCost,
            TotalLaborCost = totalLaborCost,
            TotalResourcesCost = totalResourcesCost,
            InventoryValue = inventoryValue,

            CompletedFieldWorksCount = fieldWorks.Count(x => x.Status == FieldWorkStatus.Completed),
            PlannedFieldWorksCount = fieldWorks.Count(x => x.Status == FieldWorkStatus.Planned),
            InProgressFieldWorksCount = fieldWorks.Count(x => x.Status == FieldWorkStatus.InProgress),
            CancelledFieldWorksCount = fieldWorks.Count(x => x.Status == FieldWorkStatus.Cancelled),

            CostsByWorkType = costsByWorkType,
            CostsByField = costsByField,
            InventoryValueByCategory = inventoryValueByCategory,
            FieldWorksByStatus = fieldWorksByStatus,
            MostExpensiveFieldWorks = mostExpensiveFieldWorks
        };

        return View(viewModel);
    }

    private static decimal CalculatePercentage(decimal value, decimal total)
    {
        if (total <= 0)
        {
            return 0;
        }

        return Math.Round(value / total * 100, 2);
    }

    private static decimal CalculatePercentage(int value, int total)
    {
        if (total <= 0)
        {
            return 0;
        }

        return Math.Round((decimal)value / total * 100, 2);
    }
}