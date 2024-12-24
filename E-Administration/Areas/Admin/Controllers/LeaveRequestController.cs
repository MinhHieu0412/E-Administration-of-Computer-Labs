using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

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

        public IActionResult ApproveMakeUp(int id)
        {
            var makeUpRequest = _context.MakeUpRequests.Find(id);
            if (makeUpRequest != null)
            {
                makeUpRequest.IsApproved = true;
                makeUpRequest.Feedback = null; // Xóa phản hồi nếu trước đó đã bị từ chối
                _context.SaveChanges();
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

            var makeUpRequest = _context.MakeUpRequests.Find(id);
            if (makeUpRequest != null)
            {
                makeUpRequest.IsApproved = false;
                makeUpRequest.Feedback = feedback; // Lưu phản hồi từ Admin
                _context.SaveChanges();
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn xin dạy bù.";
            }

            return RedirectToAction("Index");
        }
    }
}
