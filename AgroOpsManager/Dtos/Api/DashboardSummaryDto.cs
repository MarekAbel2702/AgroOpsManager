namespace AgroOpsManager.Web.Dtos.Api;

public class DashboardSummaryDto
{
    public int ActiveFieldsCount { get; set; }

    public int MachinesCount { get; set; }

    public int MachinesRequiringServiceCount { get; set; }

    public int PlannedFieldWorksCount { get; set; }

    public int CompletedFieldWorksCount { get; set; }

    public int LowStockItemsCount { get; set; }

    public decimal TotalFieldWorksCost { get; set; }

    public decimal InventoryValue { get; set; }
}