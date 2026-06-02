using AgroOpsManager.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroOpsManager.Core.Entities
{
    public class FieldWork : BaseEntity
    {
        public int FieldId { get; set; }
        public Field Field { get; set; } = default!;
        public int? MachineId { get; set; }
        public Machine? Machine { get; set; }
        public FieldWorkType Type { get; set; }
        public DateTime PlannedDate { get; set; }
        public DateTime? StartedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }
        public FieldWorkStatus Status { get; set; } = FieldWorkStatus.Planned;
        public decimal LaborCost { get; set; }
        public string? OperatorName { get; set; }
        public string? Notes { get; set; }
        public ICollection<WorkResourceUsage> ResourceUsages { get; set; } = new List<WorkResourceUsage>();
        public decimal CalculateResourcesCost()
        {
            return ResourceUsages.Sum(x => x.TotalCost);
        }

        public decimal CalculateTotalCost()
        {
            return LaborCost + CalculateResourcesCost();
        }
    }
}
