using AgroOpsManager.Core.Entities;

namespace AgroOpsManager.Web.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int ActiveFieldsCount { get; set; }

    public int MachinesCount { get; set; }

    public int MachinesRequiringServiceCount { get; set; }

    public int PlannedFieldWorksCount { get; set; }

    public int CompletedFieldWorksCount { get; set; }

    public int LowStockItemsCount { get; set; }

    public decimal TotalFieldWorksCost { get; set; }

    public decimal InventoryValue { get; set; }

    public List<FieldWork> UpcomingFieldWorks { get; set; } = new();

    public List<FieldWork> RecentlyCompletedFieldWorks { get; set; } = new();

    public List<Machine> MachinesRequiringService { get; set; } = new();

    public List<InventoryItem> LowStockItems { get; set; } = new();
}