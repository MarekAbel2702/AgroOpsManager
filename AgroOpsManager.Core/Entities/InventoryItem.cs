using AgroOpsManager.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroOpsManager.Core.Entities
{
    public class InventoryItem : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public InventoryCategory Category { get; set; }
        public string Unit { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal MinimumQuantity { get; set; }
        public decimal UnitPrice { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public string? SupplierName { get; set; }
        public string? Notes { get; set; }
        public ICollection<WorkResourceUsage> ResourceUsages { get; set; } = new List<WorkResourceUsage>();
        public bool IsLowStock()
        {
            return Quantity <= MinimumQuantity;
        }
    }
}
