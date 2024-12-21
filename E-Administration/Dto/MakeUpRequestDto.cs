using System;
using System.ComponentModel.DataAnnotations;

namespace E_Administration.Dto
{
    public class MakeUpRequestDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Bạn cần chọn đơn xin nghỉ.")]
        public int LeaveRequestId { get; set; }

        [Required(ErrorMessage = "Bạn cần chọn phòng.")]
        public int LabId { get; set; }

        [Required(ErrorMessage = "Bạn cần nhập ngày dạy bù.")]
        [DataType(DataType.Date)]
        public DateTime MakeUpDate { get; set; }

        [Required(ErrorMessage = "Bạn cần chọn giờ dạy bù.")]
        [DataType(DataType.Time)]
        public TimeSpan MakeUpTime { get; set; }

        public bool IsApproved { get; set; } = false;

        public string? Feedback { get; set; } // Không cần nhập khi tạo đơn
    }
}
