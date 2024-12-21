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
                .Select(lr => new SelectListItem
                {
                    Value = lr.Id.ToString(),
                    Text = $"Nghỉ từ {lr.StartDate:dd/MM/yyyy} đến {lr.EndDate:dd/MM/yyyy}"
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
            if (!ModelState.IsValid)
            {
                ViewBag.LeaveRequests = _context.LeaveRequests
                    .Select(lr => new SelectListItem
                    {
                        Value = lr.Id.ToString(),
                        Text = $"Nghỉ từ {lr.StartDate:dd/MM/yyyy} đến {lr.EndDate:dd/MM/yyyy}"
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
