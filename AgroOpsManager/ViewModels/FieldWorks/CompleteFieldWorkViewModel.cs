using System.ComponentModel.DataAnnotations;

namespace AgroOpsManager.Web.ViewModels.FieldWorks;

public class CompleteFieldWorkViewModel
{
    public int FieldWorkId { get; set; }

    public string FieldName { get; set; } = string.Empty;

    public string WorkType { get; set; } = string.Empty;

    public DateTime PlannedDate { get; set; }

    public decimal LaborCost { get; set; }

    [StringLength(1000, ErrorMessage = "Notatki mogą mieć maksymalnie 1000 znaków.")]
    [Display(Name = "Notatki końcowe")]
    public string? CompletionNotes { get; set; }

    public List<ResourceUsageInputViewModel> Resources { get; set; } = new();
}