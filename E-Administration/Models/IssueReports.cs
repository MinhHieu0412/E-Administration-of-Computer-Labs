using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Models
{
    public class IssueReports
    {
        public int ID { get; set; }
        public int LabID { get; set; }
        public Lab? Lab { get; set; }
        public int ReporterID { get; set; }
        public User? Reporter { get; set; }
        [Column("DepartmentsID")]
        public int DepartmentID { get; set; }
        public Department? Department { get; set; }
        public string? Description { get; set; }
        [Required]
        public int EquipmentID { get; set; }
        public Department? Departments { get; set; } // Navigation property

        public Equipments? Equipments { get; set; } // Quan hệ 1-n hoặc n-1

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Pending";
        [NotMapped]
        public RepairAssignments? RepairAssignments { get; set; }

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        //[DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedAt { get; set; } // Automatically updated on changes
        
    }
}
