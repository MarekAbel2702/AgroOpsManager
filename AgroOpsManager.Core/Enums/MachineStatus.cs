using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AgroOpsManager.Core.Enums
{
    public enum MachineStatus
    {
        Available = 1,
        InUse = 2,
        InService = 3,
        Broken = 4,
        Retired = 5
    }
}
