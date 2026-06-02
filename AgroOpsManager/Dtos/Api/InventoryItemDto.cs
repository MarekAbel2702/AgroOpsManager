namespace AgroOpsManager.Web.Dtos.Api;

public class InventoryItemDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Category { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public decimal Quantity { get; set; }

    public decimal MinimumQuantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal StockValue { get; set; }

    public bool IsLowStock { get; set; }

    public decimal MissingToMinimum { get; set; }

    public DateTime? ExpirationDate { get; set; }

    public string? SupplierName { get; set; }
}