using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VesselRegistry.Api.Entities
{
    public class VesselType
    {
        [Key]
        public int VesselTypeId { get; set; } // int, PK, identity

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty; // nvarchar(50), Required, unique[cite: 2]
    }
}