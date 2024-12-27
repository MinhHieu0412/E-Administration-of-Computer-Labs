using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using System.Security.Claims;

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
            // Get the UserId of the current user
            var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");

            // Filter the leave requests list of the current user
            var leaveRequests = _context.LeaveRequests
                .Where(lr => lr.UserId == userId)
                .Select(lr => new
                {
                    lr.Id,
                    UserName = _context.Users.FirstOrDefault(u => u.ID == lr.UserId).UserName ?? "Undefined",
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
            // Get the UserId of the current user
            var userId = User.Identity.IsAuthenticated
                ? int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0")
                : 0;

            if (userId == 0)
            {
                return RedirectToAction("Login", "Account"); // Redirect if not logged in
            }

            // Retrieve user information if needed (no need to pass the list of all Users)
            var user = _context.Users.FirstOrDefault(u => u.ID == userId);
            ViewBag.UserName = user?.UserName ?? "Undefined"; // Display the user name if needed

            return View();
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LeaveRequest model)
        {
            try
            {
                // Get the UserId of the current user
                var userId = int.Parse(User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value ?? "0");
                model.UserId = userId;

                // Validate start and end dates
                if (model.StartDate <= DateTime.Now)
                {
                    ModelState.AddModelError("StartDate", "The start date must be greater than the current date.");
                }

                if (model.EndDate <= model.StartDate)
                {
                    ModelState.AddModelError("EndDate", "The end date must be greater than the start date.");
                }

                // Return the View if invalid
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                // Save to database
                _context.LeaveRequests.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // Log error (if needed)
                Console.WriteLine($"Error: {ex.Message}");

                // Display general error
                ModelState.AddModelError("", $"An error occurred while saving data: {ex.Message}");
                return View(model);
            }
        }

    }
}
