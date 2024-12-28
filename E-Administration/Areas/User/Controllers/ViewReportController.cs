using E_Administration.Data;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Areas.User.Controllers
{
    [Area("User")]
    public class ViewReportController : Controller
    {
        private readonly DemoDbContext ctx;

        public ViewReportController(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }

        public ActionResult Index()
        {
            // Get the current user's ID from the claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                return NotFound(); // Handle the case where userIdClaim is null or empty
            }

            // Convert the userIdClaim from string to int
            if (!int.TryParse(userIdClaim, out int userId))
            {
                return NotFound(); // Return a 404 if conversion fails
            }

            // Find the user by their ID (now userId is of type int)
            var currentUser = ctx.Users.SingleOrDefault(u => u.ID == userId);

            if (currentUser == null)
            {
                return NotFound(); // Return a 404 if the user is not found
            }

            // Get the department of the logged-in user
            var userDepartmentId = currentUser.DepartmentID;
            if (userDepartmentId == null)
            {
                return NotFound(); // Return a 404 if the department ID is null
            }

            // Fetch all issue reports for this department
            var reports = ctx.IssueReports
                .Where(r => r.DepartmentID == userDepartmentId)
                .Include(r => r.Reporter)  // Include the Reporter (User)
                .ToList();

            // Fetch the associated Equipments for each report based on EquipmentID
            var equipmentIds = reports.Select(r => r.EquipmentID).Distinct().ToList();
            var equipmentList = ctx.Equipments.Where(e => equipmentIds.Contains(e.ID)).ToList();

            // Optionally, you can add equipment as an anonymous object or map it into a DTO
            var reportsWithEquipment = reports.Select(r =>
            {
                // Find the equipment for the current report
                var equipment = equipmentList.FirstOrDefault(e => e.ID == r.EquipmentID);
                return new
                {
                    Report = r,
                    Equipment = equipment
                };
            }).ToList();

            return View(reportsWithEquipment); // Pass the reports with equipment to the view
        }
    }
}
