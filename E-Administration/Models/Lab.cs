using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Models
{
    public class Lab
    {
        public int ID { get; set; }
        [Required]
        public string? Name { get; set; }
        public int DepartmentID { get; set; }

        [NotMapped]
        public Department? Department { get; set; }

        [Required]
        [StringLength(500)]
        public string? Description { get; set; }
        [Required]
        [StringLength(100)]
        public string? Location { get; set; }
        [Required]
        public int? Capacity { get; set; }   
        [Required]
        public bool? IsOperational { get; set; } = true;

        
        public ICollection<Assignments>? Assignments { get; set; }
        public ICollection<IssueReports>? IssueReports { get; set; }
        public ICollection<Equipments>? Equipments { get; set; }
        
        public DateTime? CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        public DateTime? UpdatedAt { get; set; } // Automatically updated on changes
       
    }
}
