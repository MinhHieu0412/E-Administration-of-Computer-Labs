using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Models
{
    public class LabRequests
    {
        public int ID { get; set; }
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        public int RequestedByID { get; set; }
        public User RequestedBy { get; set; }
        public string Purpose { get; set; }
        public string AdminResponse { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } // Automatically updated on changes
    }
}
