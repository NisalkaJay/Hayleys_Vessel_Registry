using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VesselRegistry.Api.Entities
{
    public class Vessel
    {
        [Key]
        public int VesselId { get; set; } // int, PK, identity[cite: 2]

        public int CompanyId { get; set; } // Required, taken from header later[cite: 2]

        [Required]
        [MaxLength(100)]
        public string VesselName { get; set; } = string.Empty; // nvarchar(100), Required

        [Required]
        [StringLength(7, MinimumLength = 7)]
        [Column(TypeName = "char(7)")]
        public string ImoNumber { get; set; } = string.Empty; // char(7), Required, exactly 7 digits[cite: 3]

        public int VesselTypeId { get; set; } // FK to VesselType[cite: 3]
        public VesselType VesselType { get; set; } = null!;

        [Required]
        [MaxLength(60)]
        public string FlagCountry { get; set; } = string.Empty; // nvarchar(60), Required[cite: 3]

        [Column(TypeName = "decimal(12,2)")]
        public decimal GrossTonnage { get; set; } // decimal(12,2), Required, more than 0[cite: 3]

        public int YearBuilt { get; set; } // Required, between 1950 and current year[cite: 3]

        public bool IsActive { get; set; } = true; // Defaults to 1[cite: 3]

        public int CreatedBy { get; set; } // From X-User-Id[cite: 3]
        public DateTime CreatedAt { get; set; } // UTC time[cite: 3]

        public int? ModifiedBy { get; set; } // Nullable[cite: 3]
        public DateTime? ModifiedAt { get; set; } // Nullable[cite: 3]
    }
}