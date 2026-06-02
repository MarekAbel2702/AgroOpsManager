namespace AgroOpsManager.Web.Dtos.Api;

public class MachineDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string SerialNumber { get; set; } = string.Empty;

    public int ProductionYear { get; set; }

    public int CurrentWorkingHours { get; set; }

    public int WorkingHoursAtLastService { get; set; }

    public int ServiceIntervalHours { get; set; }

    public int HoursSinceLastService { get; set; }

    public int? ExceededServiceHoursBy { get; set; }

    public bool RequiresService { get; set; }

    public string Status { get; set; } = string.Empty;
}