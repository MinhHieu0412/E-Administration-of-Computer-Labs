using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin/[controller]")]
    public class RepairController : Controller
    {
        private readonly DemoDbContext _dbContext;

        public RepairController(DemoDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        // GET: /Admin/Repair/Index
        [HttpGet("Index")]
        public IActionResult Index(string status = "All", string searchByID = "", string searchByName = "", string sortOrder = "asc")
        {
            IQueryable<IssueReports> reportsQuery = _dbContext.IssueReports
                .Include(ir => ir.Lab)
                .Include(ir => ir.Department)
                .Include(ir => ir.Reporter);

            // Apply filter by status
            if (status != "All")
            {
                reportsQuery = reportsQuery.Where(ir => ir.Status == status);
            }

            // Apply search filters
            if (!string.IsNullOrWhiteSpace(searchByID))
            {
                reportsQuery = reportsQuery.Where(ir => ir.ID.ToString().Contains(searchByID));
            }
            if (!string.IsNullOrWhiteSpace(searchByName))
            {
                reportsQuery = reportsQuery.Where(ir => ir.Reporter.UserName.Contains(searchByName));
            }

            // Apply sorting
            reportsQuery = sortOrder == "asc" ? reportsQuery.OrderBy(ir => ir.ID) : reportsQuery.OrderByDescending(ir => ir.ID);

            var reports = reportsQuery.ToList();

            ViewData["CurrentStatus"] = status;
            ViewData["SearchByID"] = searchByID;
            ViewData["SearchByName"] = searchByName;
            ViewData["SortOrder"] = sortOrder;

            return View(reports);
        }



        [HttpPost("Approve/{id}")]
        public IActionResult ApproveConfirmed(int id)
        {
            var report = _dbContext.IssueReports.FirstOrDefault(r => r.ID == id);

            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction("Index");
            }

            // Update the report status to "Approved"
            report.Status = "Approved";
            report.UpdatedAt = DateTime.Now;
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = $"Report {id} approved.";
            // Redirect to Index with filter set to "Approved"
            return RedirectToAction("Index", new { status = "Approved" });
        }


        // GET: /Admin/Repair/Details/{id}
        [HttpGet("Details/{id}")]
        public IActionResult Details(int id)
        {
            var report = _dbContext.IssueReports
                .Include(ir => ir.Lab)
                .Include(ir => ir.Department)
                .Include(ir => ir.Reporter)
                .Include(ir => ir.Equipments) // Bao gồm thông tin Equipment
                .FirstOrDefault(ir => ir.ID == id);

            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction("Index");
            }

            return View(report);
        }



    }
}
