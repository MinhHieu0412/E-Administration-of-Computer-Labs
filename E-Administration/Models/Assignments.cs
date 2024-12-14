using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Models
{
    public class Assignments
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public User User { get; set; }
        public int LabID { get; set; }
        public Lab Lab { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan TimeStart { get; set; }
        public TimeSpan TimeEnd { get; set; }
        public string Notes { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? CreatedAt { get; set; } = DateTime.Now; // Automatically set to current timestamp

        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime? UpdatedAt { get; set; } // Automatically updated on changes
    }
}
