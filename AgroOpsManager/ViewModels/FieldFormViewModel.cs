using AgroOpsManager.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace AgroOpsManager.Web.ViewModels
{
    public class FieldFormViewModel
    {
        public int? Id { get; set; }

        [Required(ErrorMessage = "Nazwa pola jest wymagana.")]
        [StringLength(120, ErrorMessage = "Nazwa pola może mieć maksymalnie 120 znaków.")]
        [Display(Name = "Nazwa pola")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lokalizacja jest wymagana.")]
        [StringLength(200, ErrorMessage = "Lokalizacja może mieć maksymalnie 200 znaków.")]
        [Display(Name = "Lokalizacja")]
        public string Location { get; set; } = string.Empty;

        [Range(0.01, 10000, ErrorMessage = "Powierzchnia musi być większa od 0.")]
        [Display(Name = "Powierzchnia w hektarach")]
        public decimal AreaInHectares { get; set; }

        [Display(Name = "Typ gleby")]
        public SoilType SoilType { get; set; } = SoilType.Unknown;

        [Display(Name = "Aktualna uprawa")]
        public CropType CurrentCrop { get; set; } = CropType.None;

        [Display(Name = "Status")]
        public FieldStatus Status { get; set; } = FieldStatus.Active;

        [StringLength(1000, ErrorMessage = "Notatki mogą mieć maksymalnie 1000 znaków.")]
        [Display(Name = "Notatki")]
        public string? Notes { get; set;}
    }
}
