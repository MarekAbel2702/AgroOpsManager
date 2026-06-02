using AgroOpsManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace AgroOpsManager.Web.ViewModels.Inventory;

public class InventoryItemFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Nazwa pozycji magazynowej jest wymagana.")]
    [StringLength(120, ErrorMessage = "Nazwa może mieć maksymalnie 120 znaków.")]
    [Display(Name = "Nazwa")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Kategoria")]
    public InventoryCategory Category { get; set; } = InventoryCategory.Other;

    [Required(ErrorMessage = "Jednostka jest wymagana.")]
    [StringLength(30, ErrorMessage = "Jednostka może mieć maksymalnie 30 znaków.")]
    [Display(Name = "Jednostka")]
    public string Unit { get; set; } = string.Empty;

    [Range(0, 100000000, ErrorMessage = "Ilość nie może być ujemna.")]
    [Display(Name = "Aktualna ilość")]
    public decimal Quantity { get; set; }

    [Range(0, 100000000, ErrorMessage = "Minimalna ilość nie może być ujemna.")]
    [Display(Name = "Minimalny stan")]
    public decimal MinimumQuantity { get; set; }

    [Range(0, 100000000, ErrorMessage = "Cena jednostkowa nie może być ujemna.")]
    [Display(Name = "Cena jednostkowa")]
    public decimal UnitPrice { get; set; }

    [Display(Name = "Data ważności")]
    [DataType(DataType.Date)]
    public DateTime? ExpirationDate { get; set; }

    [StringLength(160, ErrorMessage = "Nazwa dostawcy może mieć maksymalnie 160 znaków.")]
    [Display(Name = "Dostawca")]
    public string? SupplierName { get; set; }

    [StringLength(1000, ErrorMessage = "Notatki mogą mieć maksymalnie 1000 znaków.")]
    [Display(Name = "Notatki")]
    public string? Notes { get; set; }
}