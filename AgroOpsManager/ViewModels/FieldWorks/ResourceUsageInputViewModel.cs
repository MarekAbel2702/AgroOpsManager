using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AgroOpsManager.Web.ViewModels.FieldWorks;

public class ResourceUsageInputViewModel
{
    [Display(Name = "Materiał")]
    public int? InventoryItemId { get; set; }

    [Range(0, 100000000, ErrorMessage = "Ilość nie może być ujemna.")]
    [Display(Name = "Zużyta ilość")]
    public decimal QuantityUsed { get; set; }

    public IEnumerable<SelectListItem> InventoryItems { get; set; } = new List<SelectListItem>();
}