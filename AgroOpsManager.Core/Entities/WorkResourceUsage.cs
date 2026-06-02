using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroOpsManager.Core.Entities
{
    public class WorkResourceUsage : BaseEntity
    {
        public int FieldWorkId { get; set; }
        public FieldWork FieldWork { get; set; } = default!;
        public int InventoryItemId { get; set; }
        public InventoryItem InventoryItem { get; set; } = default!;
        public decimal QuantityUsed { get; set; }
        public decimal UnitPriceAtUsage { get; set; }
        public decimal TotalCost => QuantityUsed * UnitPriceAtUsage;
    }
}
