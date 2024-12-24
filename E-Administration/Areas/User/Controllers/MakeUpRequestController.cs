using E_Administration.Data;
using E_Administration.Dto;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace E_Administration.Areas.User.Controllers
{
    [Area("User")]
    public class MakeUpRequestController : Controller
    {
        private readonly DemoDbContext _context;

        public MakeUpRequestController(DemoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh sách đơn dạy bù và kết nối các bảng liên quan
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

            // Truyền dữ liệu vào ViewBag
            ViewBag.MakeUpRequests = makeUpRequests;

            return View();
        }

        public IActionResult Create()
        {
            ViewBag.LeaveRequests = _context.LeaveRequests
                .Join(_context.Users,
                      lr => lr.UserId,
                      u => u.ID,
                      (lr, u) => new SelectListItem
                      {
                          Value = lr.Id.ToString(),
                          Text = $"{u.UserName} - Nghỉ từ {lr.StartDate:dd/MM/yyyy} đến {lr.EndDate:dd/MM/yyyy}"
                      })
                .ToList();

            ViewBag.Labs = _context.Labs
                .Select(l => new SelectListItem
                {
                    Value = l.ID.ToString(),
                    Text = l.Name
                })
                .ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(MakeUpRequestDto model)
        {
            // Lấy thông tin của đơn nghỉ liên quan
            var leaveRequest = _context.LeaveRequests.FirstOrDefault(lr => lr.Id == model.LeaveRequestId);

            // Kiểm tra logic
            if (leaveRequest != null && model.MakeUpDate <= leaveRequest.EndDate)
            {
                ModelState.AddModelError("MakeUpDate", "Ngày dạy bù phải lớn hơn ngày kết thúc nghỉ.");
            }

            if (!ModelState.IsValid)
            {
                // Reload danh sách nếu có lỗi
                ViewBag.LeaveRequests = _context.LeaveRequests
                    .Join(_context.Users,
                          lr => lr.UserId,
                          u => u.ID,
                          (lr, u) => new SelectListItem
                          {
                              Value = lr.Id.ToString(),
                              Text = $"{u.UserName} - Nghỉ từ {lr.StartDate:dd/MM/yyyy} đến {lr.EndDate:dd/MM/yyyy}"
                          })
                    .ToList();

                ViewBag.Labs = _context.Labs
                    .Select(l => new SelectListItem
                    {
                        Value = l.ID.ToString(),
                        Text = l.Name
                    })
                    .ToList();

                return View(model);
            }

            // Lưu dữ liệu nếu hợp lệ
            var makeUpRequest = new MakeUpRequest
            {
                LeaveRequestId = model.LeaveRequestId,
                LabId = model.LabId,
                MakeUpDate = model.MakeUpDate,
                MakeUpTime = model.MakeUpTime,
                IsApproved = false
            };

            _context.MakeUpRequests.Add(makeUpRequest);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
