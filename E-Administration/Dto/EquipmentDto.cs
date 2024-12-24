using E_Administration.Models;
using System.ComponentModel.DataAnnotations;

namespace E_Administration.Dto
{
    public class EquipmentDto
    {
        public int ID { get; set; }
        [Required]
        public int LabID { get; set; }
        [Required]
        [StringLength(100)]
        public string Name { get; set; }
        [Required]
        [StringLength(50)]
        public string SerialNumber { get; set; }
        [Required]
        [StringLength(200)]
        public string EquipmentDetails { get; set; }
        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Operational";
        public DateTime? CreatedAt { get; set; } // Automatically set to current timestamp

        public DateTime? UpdatedAt { get; set; } // Automatically updated on changes
    }
}
