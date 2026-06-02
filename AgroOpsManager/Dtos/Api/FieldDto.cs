namespace AgroOpsManager.Web.Dtos.Api;

public class FieldDto
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal AreaInHectares { get; set; }

    public string Location { get; set; } = string.Empty;

    public string SoilType { get; set; } = string.Empty;

    public string CurrentCrop { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public string? Notes { get; set; }
}