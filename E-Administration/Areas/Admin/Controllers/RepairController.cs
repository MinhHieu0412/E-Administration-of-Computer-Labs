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
            // Start query with necessary includes
            IQueryable<IssueReports> reportsQuery = _dbContext.IssueReports
                .Include(ir => ir.Lab)            // Include Lab navigation property
                .Include(ir => ir.Department)     //
                                                  // Include Department navigation property
                .Include(ir => ir.Reporter);      // Include Reporter navigation property

            // Apply status filter
            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                reportsQuery = reportsQuery.Where(ir => ir.Status == status);
            }

            // Apply search filters (with null checks for navigation properties)
            if (!string.IsNullOrWhiteSpace(searchByID))
            {
                reportsQuery = reportsQuery.Where(ir => ir.ID.ToString().Contains(searchByID));
            }
            if (!string.IsNullOrWhiteSpace(searchByName))
            {
                reportsQuery = reportsQuery.Where(ir => ir.Reporter != null && ir.Description.Contains(searchByName));
            }

            // Apply sorting
            reportsQuery = sortOrder.ToLower() == "asc"
                ? reportsQuery.OrderBy(ir => ir.ID)
                : reportsQuery.OrderByDescending(ir => ir.ID);

            // Execute the query
            var reports = reportsQuery.ToList();

            // Pass data to the view
            ViewBag.CurrentStatus = status;
            ViewData["SearchByID"] = searchByID;
            ViewData["SearchByName"] = searchByName;
            ViewData["SortOrder"] = sortOrder;

            return View(reports); // Pass IssueReports to the view
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
        [HttpPost("Approve/{id}")]
        public IActionResult Approve(int id)
        {
            // Lấy báo cáo từ cơ sở dữ liệu
            var report = _dbContext.IssueReports.FirstOrDefault(r => r.ID == id);

            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction("Index");
            }

            // Cập nhật trạng thái
            report.Status = "Approved";
            report.UpdatedAt = DateTime.Now;

            // Lưu thay đổi
            _dbContext.SaveChanges();

            // Thông báo thành công
            TempData["SuccessMessage"] = $"Report {id} has been approved.";
            return RedirectToAction("Index");
        }

        [HttpGet("ApprovePage/{id}")]
        public IActionResult ApprovePage(int id)
        {
            var report = _dbContext.IssueReports.FirstOrDefault(r => r.ID == id);

            if (report == null)
            {
                TempData["ErrorMessage"] = "Report not found.";
                return RedirectToAction("Index");
            }

            return View(report); // Pass the report to the ApprovePage view
        }







    }
}
