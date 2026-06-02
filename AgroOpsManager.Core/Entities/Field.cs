using AgroOpsManager.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroOpsManager.Core.Entities
{
    public class Field : BaseEntity
    {
        public string Name { get; set; } = string.Empty;
        public decimal AreaInHectares { get; set; }
        public string Location { get; set; } = string.Empty;
        public SoilType SoilType { get; set; }
        public CropType CurrentCrop { get; set; }
        public FieldStatus Status { get; set; } = FieldStatus.Active;
        public string? Notes { get; set; }
        public ICollection<FieldWork> FieldWorks { get; set; } = new List<FieldWork>();
    }
}
