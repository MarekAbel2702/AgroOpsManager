using AgroOpsManager.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroOpsManager.Core.Entities
{
    public class Machine : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public MachineType Type { get; set; }
        public string SerialNumber { get; set; } = string.Empty;
        public int ProductionYear { get; set; } 
        public int CurrentWorkingHours { get; set; }
        public int ServiceIntervalHours { get; set; }
        public int WorkingHoursAtLastService { get; set; }
        public MachineStatus Status { get; set; } = MachineStatus.Available;
        public string? Notes { get; set; }
        public ICollection<FieldWork> FieldWorks { get; set; } = new List<FieldWork>();
        public bool RequiresService()
        {
            return CurrentWorkingHours - WorkingHoursAtLastService >= ServiceIntervalHours;
        }
    }
}
