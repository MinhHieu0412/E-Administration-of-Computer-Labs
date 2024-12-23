using E_Administration.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class MakeUpRequest
{
    [Key]
    public int Id { get; set; }

    [Required]
    [ForeignKey("LeaveRequest")]
    public int LeaveRequestId { get; set; }

    [Required]
    [ForeignKey("Lab")]
    public int LabId { get; set; }

    [Required]
    [DataType(DataType.Date)]
    public DateTime MakeUpDate { get; set; }

    [Required]
    [DataType(DataType.Time)]
    public TimeSpan MakeUpTime { get; set; }

    public bool IsApproved { get; set; } = false;

    public string? Feedback { get; set; } // Nullable để cho phép không có phản hồi

    public LeaveRequest LeaveRequest { get; set; }
    public Lab Lab { get; set; }
}
