namespace AgroOpsManager.Web.Dtos.Api;

public class FieldWorkDto
{
    public int Id { get; set; }

    public int FieldId { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public int? MachineId { get; set; }

    public string? MachineName { get; set; }

    public string Type { get; set; } = string.Empty;

    public DateTime PlannedDate { get; set; }

    public DateTime? StartedAtUtc { get; set; }

    public DateTime? CompletedAtUtc { get; set; }

    public string Status { get; set; } = string.Empty;

    public decimal LaborCost { get; set; }

    public decimal ResourcesCost { get; set; }

    public decimal TotalCost { get; set; }

    public string? OperatorName { get; set; }
}