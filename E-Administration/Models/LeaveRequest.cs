using System;
using System.ComponentModel.DataAnnotations;

namespace E_Administration.Models
{
    public class LeaveRequest
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; } // ID của người dùng tạo đơn

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } // Ngày bắt đầu nghỉ

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } // Ngày kết thúc nghỉ

        [MaxLength(500)]
        public string Reason { get; set; } // Lý do nghỉ

        public bool IsApproved { get; set; } // Trạng thái duyệt (true/false)
    }
}
