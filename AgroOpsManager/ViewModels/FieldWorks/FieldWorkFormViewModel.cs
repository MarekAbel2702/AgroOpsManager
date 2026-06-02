using AgroOpsManager.Core.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AgroOpsManager.Web.ViewModels.FieldWorks;

public class FieldWorkFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Pole jest wymagane.")]
    [Display(Name = "Pole")]
    public int FieldId { get; set; }

    [Display(Name = "Maszyna")]
    public int? MachineId { get; set; }

    [Display(Name = "Typ pracy")]
    public FieldWorkType Type { get; set; } = FieldWorkType.Other;

    [Required(ErrorMessage = "Data planowana jest wymagana.")]
    [DataType(DataType.Date)]
    [Display(Name = "Data planowana")]
    public DateTime PlannedDate { get; set; } = DateTime.UtcNow.Date;

    [Display(Name = "Status")]
    public FieldWorkStatus Status { get; set; } = FieldWorkStatus.Planned;

    [Range(0, 100000000, ErrorMessage = "Koszt robocizny nie może być ujemny.")]
    [Display(Name = "Koszt robocizny")]
    public decimal LaborCost { get; set; }

    [StringLength(120, ErrorMessage = "Nazwa operatora może mieć maksymalnie 120 znaków.")]
    [Display(Name = "Operator")]
    public string? OperatorName { get; set; }

    [StringLength(1000, ErrorMessage = "Notatki mogą mieć maksymalnie 1000 znaków.")]
    [Display(Name = "Notatki")]
    public string? Notes { get; set; }

    public IEnumerable<SelectListItem> Fields { get; set; } = new List<SelectListItem>();

    public IEnumerable<SelectListItem> Machines { get; set; } = new List<SelectListItem>();
}