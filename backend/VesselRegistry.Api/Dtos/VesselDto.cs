using System.ComponentModel.DataAnnotations;

namespace VesselRegistry.Api.Dtos
{
    public class VesselDto
    {
        public int VesselId { get; set; }

        [Required(ErrorMessage = "Vessel name is required")]
        [StringLength(100, ErrorMessage = "Vessel name cannot exceed 100 characters")]
        public string VesselName { get; set; } = string.Empty;

        [Required(ErrorMessage = "IMO number is required")]
        [RegularExpression(@"^\d{7}$", ErrorMessage = "IMO number must be exactly 7 digits")]
        public string ImoNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Vessel type is required")]
        public int VesselTypeId { get; set; }

        public string? VesselTypeName { get; set; } // Read-only for list views

        [Required(ErrorMessage = "Flag country is required")]
        [StringLength(60, ErrorMessage = "Flag country cannot exceed 60 characters")]
        public string FlagCountry { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gross tonnage is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Gross tonnage must be more than 0")]
        public decimal GrossTonnage { get; set; }

        [Required(ErrorMessage = "Year built is required")]
        [Range(1950, 2100, ErrorMessage = "Year built must be between 1950 and the current year")]
        public int YearBuilt { get; set; }

        public bool IsActive { get; set; } = true;
    }
}