using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Models
{
    public class Department
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public ICollection<User> Users { get; set; }
        public ICollection<Lab> Labs { get; set; }
        public ICollection<LabRequests> LabRequests { get; set; }
        public ICollection<IssueReports> IssueReports { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedAt { get; set; } // Automatically updated on changes
    }
}
