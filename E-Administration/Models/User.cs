using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Models
{
    public class User
    {
        [Key]
        public int ID { get; set; }
        public string? UserName { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Image { get; set; }
        public string? Role { get; set; }
        public bool? Status { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedAt { get; set; } // Automatically updated on changes
        public int DepartmentID { get; set; }
        public Department Department { get; set; }
        public ICollection<Assignments> Assignments { get; set; }
        public ICollection<IssueReports> IssueReports { get; set; }
        public ICollection<LabRequests> LabRequests { get; set; }
    }
}
