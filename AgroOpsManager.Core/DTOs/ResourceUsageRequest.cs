namespace AgroOpsManager.Core.DTOs;

public class ResourceUsageRequest
{
    public int InventoryItemId { get; set; }

    public decimal QuantityUsed { get; set; }
}