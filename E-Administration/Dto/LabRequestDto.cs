using E_Administration.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Dto
{
    public class LabRequestDto
    {
        public int ID { get; set; }
        [Required]
        public int DepartmentID { get; set; }
        [Required]

        public int RequestedByID { get; set; }
        [Required]

        public string Purpose { get; set; }
        
        public string? AdminResponse { get; set; }
        public string Status { get; set; } = "Pending";
       
        public DateTime? CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        public DateTime? UpdatedAt { get; set; } // Automatically updated on changes
    }
}
