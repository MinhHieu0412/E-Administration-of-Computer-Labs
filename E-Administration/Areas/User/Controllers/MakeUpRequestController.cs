using E_Administration.Data;
using E_Administration.Dto;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Security.Claims;

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
            // Retrieve the list of make-up requests and join related tables
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

            // Pass data to ViewBag
            ViewBag.MakeUpRequests = makeUpRequests;

            return View();
        }

        public IActionResult Create()
        {
            // Get the UserId of the current user
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Filter the leave requests of the current user
            ViewBag.LeaveRequests = _context.LeaveRequests
                .Where(lr => lr.UserId == userId)
                .Join(_context.Users,
                      lr => lr.UserId,
                      u => u.ID,
                      (lr, u) => new SelectListItem
                      {
                          Value = lr.Id.ToString(),
                          Text = $"{u.UserName} - Leave from {lr.StartDate:dd/MM/yyyy} to {lr.EndDate:dd/MM/yyyy}"
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
            // Get the UserId of the current user
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Retrieve the related leave request
            var leaveRequest = _context.LeaveRequests.FirstOrDefault(lr => lr.Id == model.LeaveRequestId && lr.UserId == userId);

            // Validate logic
            if (leaveRequest == null)
            {
                ModelState.AddModelError("LeaveRequestId", "Invalid leave request.");
            }
            else if (model.MakeUpDate <= leaveRequest.EndDate)
            {
                ModelState.AddModelError("MakeUpDate", "The make-up date must be after the leave end date.");
            }

            if (!ModelState.IsValid)
            {
                // Reload lists if there are errors
                ViewBag.LeaveRequests = _context.LeaveRequests
                    .Where(lr => lr.UserId == userId)
                    .Join(_context.Users,
                          lr => lr.UserId,
                          u => u.ID,
                          (lr, u) => new SelectListItem
                          {
                              Value = lr.Id.ToString(),
                              Text = $"{u.UserName} - Leave from {lr.StartDate:dd/MM/yyyy} to {lr.EndDate:dd/MM/yyyy}"
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

            // Create a new make-up request
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
