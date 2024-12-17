using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace E_Administration.Models
{
    public class Equipments
    {
        public int ID { get; set; }
        public int LabID { get; set; }
        public Lab Lab { get; set; }
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
        public DateTime? CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        public DateTime? UpdatedAt { get; set; } // Automatically updated on changes
    }
}
