using AgroOpsManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace AgroOpsManager.Web.ViewModels.Machines;

public class MachineFormViewModel
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Nazwa maszyny jest wymagana.")]
    [StringLength(120, ErrorMessage = "Nazwa maszyny może mieć maksymalnie 120 znaków.")]
    [Display(Name = "Nazwa maszyny")]
    public string Name { get; set; } = string.Empty;

    [Display(Name = "Typ maszyny")]
    public MachineType Type { get; set; } = MachineType.Tractor;

    [Required(ErrorMessage = "Numer seryjny jest wymagany.")]
    [StringLength(80, ErrorMessage = "Numer seryjny może mieć maksymalnie 80 znaków.")]
    [Display(Name = "Numer seryjny")]
    public string SerialNumber { get; set; } = string.Empty;

    [Range(1950, 2100, ErrorMessage = "Rok produkcji musi być poprawny.")]
    [Display(Name = "Rok produkcji")]
    public int ProductionYear { get; set; } = DateTime.UtcNow.Year;

    [Range(0, 1000000, ErrorMessage = "Aktualna liczba motogodzin nie może być ujemna.")]
    [Display(Name = "Aktualne motogodziny")]
    public int CurrentWorkingHours { get; set; }

    [Range(1, 1000000, ErrorMessage = "Interwał serwisowy musi być większy od 0.")]
    [Display(Name = "Interwał serwisowy")]
    public int ServiceIntervalHours { get; set; } = 250;

    [Range(0, 1000000, ErrorMessage = "Motogodziny przy ostatnim serwisie nie mogą być ujemne.")]
    [Display(Name = "Motogodziny przy ostatnim serwisie")]
    public int WorkingHoursAtLastService { get; set; }

    [Display(Name = "Status")]
    public MachineStatus Status { get; set; } = MachineStatus.Available;

    [StringLength(1000, ErrorMessage = "Notatki mogą mieć maksymalnie 1000 znaków.")]
    [Display(Name = "Notatki")]
    public string? Notes { get; set; }
}