using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MimeKit;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Security.Claims;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LeaveRequestController : Controller
    {
        private readonly DemoDbContext _context;

        public LeaveRequestController(DemoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh sách đơn xin nghỉ
            var leaveRequests = _context.LeaveRequests
                .Join(_context.Users,
                      lr => lr.UserId,
                      u => u.ID,
                      (lr, u) => new
                      {
                          lr.Id,
                          UserName = u.UserName,
                          lr.StartDate,
                          lr.EndDate,
                          lr.Reason,
                          lr.IsApproved,
                          lr.Feedback // Thêm lý do từ chối
                      })
                .ToList();

            // Lấy danh sách đơn xin dạy bù
            var makeUpRequests = _context.MakeUpRequests
                .Join(_context.LeaveRequests,
                      mr => mr.LeaveRequestId,
                      lr => lr.Id,
                      (mr, lr) => new { mr, lr })
                .Join(_context.Users,
                      combined => combined.lr.UserId,
                      u => u.ID,
                      (combined, u) => new
                      {
                          combined.mr.Id,
                          UserName = u.UserName,
                          LeaveRequestStartDate = combined.lr.StartDate,
                          LeaveRequestEndDate = combined.lr.EndDate,
                          LabName = combined.mr.Lab.Name,
                          combined.mr.MakeUpDate,
                          combined.mr.MakeUpTime,
                          combined.mr.IsApproved,
                          combined.mr.Feedback
                      })
                .ToList();

            // Truyền cả hai danh sách vào View thông qua ViewBag
            ViewBag.LeaveRequests = leaveRequests;
            ViewBag.MakeUpRequests = makeUpRequests;

            return View();
        }

        public IActionResult ApproveLeave(int id)
        {
            var leaveRequest = _context.LeaveRequests.Find(id);
            if (leaveRequest != null)
            {
                leaveRequest.IsApproved = true;
                leaveRequest.Feedback = null; // Xóa phản hồi nếu trước đó đã bị từ chối
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RejectLeave(int id, string feedback)
        {
            if (string.IsNullOrEmpty(feedback))
            {
                TempData["ErrorMessage"] = "Lý do từ chối không được để trống.";
                return RedirectToAction("Index");
            }

            var leaveRequest = _context.LeaveRequests.Find(id);
            if (leaveRequest != null)
            {
                leaveRequest.IsApproved = false;
                leaveRequest.Feedback = feedback; // Lưu phản hồi từ Admin
                _context.SaveChanges();
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn xin nghỉ.";
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult ApproveMakeUp(int id)
        {
            var makeUpRequest = _context.MakeUpRequests
                .Include(mr => mr.LeaveRequest)
                    .ThenInclude(lr => lr.User)
                .Include(mr => mr.Lab)
                .FirstOrDefault(mr => mr.Id == id);

            if (makeUpRequest != null)
            {
                makeUpRequest.IsApproved = true;
                makeUpRequest.Feedback = null; // Xóa phản hồi nếu trước đó đã bị từ chối
                _context.SaveChanges();

                // Gửi email thông báo
                SendNotificationEmail(makeUpRequest.LeaveRequest.User.Email, "Đơn dạy bù đã được duyệt",
                    $"Xin chào {makeUpRequest.LeaveRequest.User.UserName},<br><br>Đơn dạy bù của bạn vào ngày {makeUpRequest.MakeUpDate:dd/MM/yyyy} tại phòng {makeUpRequest.Lab?.Name ?? "Không xác định"} đã được duyệt.");
            }

            return RedirectToAction("Index");
        }





        [HttpPost]
        public IActionResult RejectMakeUp(int id, string feedback)
        {
            if (string.IsNullOrEmpty(feedback))
            {
                TempData["ErrorMessage"] = "Lý do từ chối không được để trống.";
                return RedirectToAction("Index");
            }

            var makeUpRequest = _context.MakeUpRequests
                .Include(mr => mr.Lab) // Include bảng Lab
                .Include(mr => mr.LeaveRequest)
                    .ThenInclude(lr => lr.User)
                .FirstOrDefault(mr => mr.Id == id);

            if (makeUpRequest != null)
            {
                makeUpRequest.IsApproved = false;
                makeUpRequest.Feedback = feedback; // Lưu phản hồi từ Admin
                _context.SaveChanges();

                // Gửi email thông báo
                SendNotificationEmail(
                    makeUpRequest.LeaveRequest.User.Email,
                    "Đơn dạy bù bị từ chối",
                    $"Xin chào {makeUpRequest.LeaveRequest.User.UserName},<br><br>Đơn dạy bù của bạn vào ngày {makeUpRequest.MakeUpDate:dd/MM/yyyy} tại phòng {makeUpRequest.Lab?.Name ?? "Không xác định"} đã bị từ chối.<br><br>Lý do: {feedback}"
                );
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn xin dạy bù.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult DeleteLeaveRequest(int id)
        {
            var leaveRequest = _context.LeaveRequests.Find(id);
            if (leaveRequest != null)
            {
                _context.LeaveRequests.Remove(leaveRequest);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Đơn xin nghỉ đã được xóa thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn xin nghỉ.";
            }

            return RedirectToAction("Index");
        }

        public IActionResult DeleteMakeUpRequest(int id)
        {
            var makeUpRequest = _context.MakeUpRequests.Find(id);
            if (makeUpRequest != null)
            {
                _context.MakeUpRequests.Remove(makeUpRequest);
                _context.SaveChanges();
                TempData["SuccessMessage"] = "Đơn xin dạy bù đã được xóa thành công.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn xin dạy bù.";
            }

            return RedirectToAction("Index");
        }
        private void SendNotificationEmail(string recipientEmail, string subject, string body)
        {
            using (var smtp = new SmtpClient("smtp.gmail.com"))
            {
                smtp.Port = 587;
                smtp.Credentials = new NetworkCredential("minhhieu114a@gmail.com", "xvgk edae kbki rbuz");
                smtp.EnableSsl = true;

                var mail = new MailMessage
                {
                    From = new MailAddress("minhhieu114a@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mail.To.Add(recipientEmail);

                try
                {
                    smtp.Send(mail);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error sending email: {ex.Message}");
                }
            }
        }

    }
}
