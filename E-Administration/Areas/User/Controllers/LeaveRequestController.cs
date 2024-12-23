using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;

namespace E_Administration.Areas.User.Controllers
{
    [Area("User")]
    public class LeaveRequestController : Controller
    {
        private readonly DemoDbContext _context;

        public LeaveRequestController(DemoDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
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
                          lr.Feedback,
                          lr.IsApproved
                      })
                .ToList();

            return View(leaveRequests);
        }

        public IActionResult Create()
        {
            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.ID.ToString(),
                    Text = u.UserName
                })
                .ToList();

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LeaveRequest model)
        {
            // Kiểm tra ngày bắt đầu
            if (model.StartDate <= DateTime.Now)
            {
                ModelState.AddModelError("StartDate", "Ngày bắt đầu nghỉ phải lớn hơn ngày hiện tại.");
            }

            // Kiểm tra ngày kết thúc
            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError("EndDate", "Ngày kết thúc nghỉ phải lớn hơn ngày bắt đầu nghỉ.");
            }

            // Nếu có lỗi, trả về View với lỗi
            if (!ModelState.IsValid)
            {
                ViewBag.Users = _context.Users
                    .Select(u => new SelectListItem
                    {
                        Value = u.ID.ToString(),
                        Text = u.UserName
                    })
                    .ToList();

                return View(model);
            }

            // Nếu hợp lệ, lưu dữ liệu
            _context.LeaveRequests.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
