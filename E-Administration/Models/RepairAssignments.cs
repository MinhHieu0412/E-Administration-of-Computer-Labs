using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Models
{
    public class RepairAssignments
    {
        public int ID { get; set; }

        public int IssueReportID { get; set; }

        public IssueReports? IssueReports { get; set; } // Navigation property đến IssueReport

        public int TechnicianID { get; set; }
        public User? Technician { get; set; } // Navigation property đến User

        public bool IsConfirmed { get; set; } // Trạng thái đã xác nhận

        public DateTime? CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        public DateTime? UpdatedAt { get; set; } = DateTime.Now; // Automatically updated on changes

        public string? Status { get; set; } // Nếu thiếu, hãy thêm dòng này

    }
}
