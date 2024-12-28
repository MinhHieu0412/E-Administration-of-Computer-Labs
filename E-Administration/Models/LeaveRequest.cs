using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace E_Administration.Models
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserId { get; set; } // ID của người dùng tạo đơn

        public User? User { get; set; } // Đánh dấu nullable để không bắt buộc phải gán giá trị

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } // Ngày bắt đầu nghỉ

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } // Ngày kết thúc nghỉ

        [MaxLength(500)]
        public string Reason { get; set; } // Lý do nghỉ

        public bool IsApproved { get; set; } // Trạng thái duyệt (true/false)

        public string? Feedback { get; set; } // Phản hồi
    }
}
