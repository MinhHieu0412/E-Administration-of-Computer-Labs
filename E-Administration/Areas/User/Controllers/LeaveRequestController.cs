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
            if (ModelState.IsValid)
            {
                _context.LeaveRequests.Add(model);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Users = _context.Users
                .Select(u => new SelectListItem
                {
                    Value = u.ID.ToString(),
                    Text = u.UserName
                })
                .ToList();

            return View(model);
        }
    }
}
