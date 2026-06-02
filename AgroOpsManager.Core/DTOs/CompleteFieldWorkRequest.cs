namespace AgroOpsManager.Core.DTOs;

public class CompleteFieldWorkRequest
{
    public int FieldWorkId { get; set; }

    public string? CompletionNotes { get; set; }

    public List<ResourceUsageRequest> Resources { get; set; } = new();
}