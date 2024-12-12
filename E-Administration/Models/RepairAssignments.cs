using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Models
{
    public class RepairAssignments
    {
        public int ID { get; set; }
        public int IssueReportID { get; set; }
        public IssueReports IssueReports { get; set; }
        public int TechnicianID { get; set; }
        public User Technician { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; } // Automatically updated on changes
    }
}
