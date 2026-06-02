namespace AgroOpsManager.Web.ViewModels.Reports;

public class ReportsViewModel
{
    public decimal TotalFieldWorksCost { get; set; }

    public decimal TotalLaborCost { get; set; }

    public decimal TotalResourcesCost { get; set; }

    public decimal InventoryValue { get; set; }

    public int CompletedFieldWorksCount { get; set; }

    public int PlannedFieldWorksCount { get; set; }

    public int InProgressFieldWorksCount { get; set; }

    public int CancelledFieldWorksCount { get; set; }

    public List<CostByTypeReportItem> CostsByWorkType { get; set; } = new();

    public List<CostByFieldReportItem> CostsByField { get; set; } = new();

    public List<InventoryValueByCategoryReportItem> InventoryValueByCategory { get; set; } = new();

    public List<FieldWorkStatusReportItem> FieldWorksByStatus { get; set; } = new();

    public List<TopFieldWorkCostReportItem> MostExpensiveFieldWorks { get; set; } = new();
}

public class CostByTypeReportItem
{
    public string WorkType { get; set; } = string.Empty;

    public int Count { get; set; }

    public decimal LaborCost { get; set; }

    public decimal ResourcesCost { get; set; }

    public decimal TotalCost { get; set; }

    public decimal Percentage { get; set; }
}

public class CostByFieldReportItem
{
    public int FieldId { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public decimal AreaInHectares { get; set; }

    public int Count { get; set; }

    public decimal TotalCost { get; set; }

    public decimal CostPerHectare { get; set; }

    public decimal Percentage { get; set; }
}

public class InventoryValueByCategoryReportItem
{
    public string Category { get; set; } = string.Empty;

    public int Count { get; set; }

    public decimal TotalValue { get; set; }

    public decimal Percentage { get; set; }
}

public class FieldWorkStatusReportItem
{
    public string Status { get; set; } = string.Empty;

    public int Count { get; set; }

    public decimal Percentage { get; set; }
}

public class TopFieldWorkCostReportItem
{
    public int FieldWorkId { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public string WorkType { get; set; } = string.Empty;

    public DateTime PlannedDate { get; set; }

    public decimal LaborCost { get; set; }

    public decimal ResourcesCost { get; set; }

    public decimal TotalCost { get; set; }

    public string Status { get; set; } = string.Empty;
}