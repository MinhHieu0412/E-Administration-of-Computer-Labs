using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Areas.User.Controllers
{

    [Area("User")]
    [Route("User/[controller]")]
    public class ReportController : Controller
    {
        private readonly DemoDbContext _dbContext;
        public ReportController(DemoDbContext dbContext)
        {
            _dbContext = dbContext; // Initialize the existing field
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }


        // GET: /Admin/Report/Report 
        [HttpGet("Report")]
        public IActionResult Report()
        {
            // Retrieve departments and labs for dropdowns
            var departments = _dbContext.Departments.Select(d => new { d.ID, d.Name }).ToList();
            var labs = _dbContext.Labs.Select(l => new { l.ID, l.Name }).ToList();


            // Pass data to ViewData
            ViewData["Departments"] = departments;
            ViewData["Labs"] = labs;

            return View("~/Areas/User/Views/Report/Report.cshtml");
        }

        [HttpGet("Submit/{id}")]
        public IActionResult ViewSubmission(int id)
        {
            var report = _dbContext.IssueReports.SingleOrDefault(i => i.ID == id);
            if (report == null)
            {
                return NotFound("Report not found.");
            }
            return View(report);
        }


        // POST: /Admin/Report/Submit
        [HttpPost("Submit")]
        public IActionResult Submit(int departmentId,
    int labId,
    int reporterId,
    int equipmentId,       // thêm tham số
    int issueLevel,        // tham số: user chọn 1–5
    string description)
        {
            try
            {

                // Kiểm tra departmentId
                var department = _dbContext.Departments.FirstOrDefault(d => d.ID == departmentId);
                if (department == null)
                {
                    // Thêm Department mặc định nếu không tồn tại
                    department = new Department
                    {
                        Name = "Default Department",
                        CreatedAt = DateTime.Now,
                        UpdatedAt = DateTime.Now
                    };

                    _dbContext.Departments.Add(department);
                    _dbContext.SaveChanges();
                    departmentId = department.ID; // Gán lại ID mới
                }




                // Kiểm tra labId
                var lab = _dbContext.Labs.FirstOrDefault(l => l.ID == labId);
                if (lab == null)
                {
                    TempData["ErrorMessage"] = "Lab not found.";
                    return RedirectToAction("ReportDetails", new { labId });
                }

                // Kiểm tra description
                if (string.IsNullOrWhiteSpace(description))
                {
                    TempData["ErrorMessage"] = "Description is required.";
                    return RedirectToAction("ReportDetails", new { labId });
                }


                // Tạo mới IssueReport
                var issueReport = new IssueReports
                {
                    DepartmentID = departmentId,
                    LabID = labId,
                    ReporterID = reporterId,
                    Description = description,
                    EquipmentID = equipmentId, // Set NULL nếu không có Equipment
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Status = "Pending"
                };

                // Lưu IssueReport vào database
                _dbContext.IssueReports.Add(issueReport);
                _dbContext.SaveChanges();

                TempData["SuccessMessage"] = "Your report has been submitted successfully.";

                // Chuyển sang trang Submit
                return View("~/Areas/User/Views/Report/Submit.cshtml", issueReport);
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException?.Message ?? ex.Message;
                Console.WriteLine("Error: " + innerException);
                TempData["ErrorMessage"] = $"Database error: {innerException}";
                return RedirectToAction("ReportDetails", new { labId });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unexpected Error: " + ex.Message);
                TempData["ErrorMessage"] = $"Unexpected error: {ex.Message}";
                return RedirectToAction("ReportDetails", new { labId });
            }


        }
        public IActionResult Submit(int id)
        {
            var report = _dbContext.IssueReports.SingleOrDefault(i => i.ID == id);
            return View("~/Areas/User/Views/Report/Submit.cshtml", report);

        }




        [HttpGet("ReportDetails")]
        public IActionResult ReportDetails(int labId)
        {
            var lab2 = _dbContext.Labs.Include(l => l.Equipments).FirstOrDefault(l => l.ID == labId);

            // Tải lab và department
            //var lab2 = _dbContext.Labs
            //    .Include(l => l.Department) // Bao gồm Department
            //    .FirstOrDefault(l => l.ID == labId);

            if (lab2 == null)
            {
                return NotFound("Lab not found.");
            }
            ViewData["LabID"] = labId;
            return View("~/Areas/User/Views/Report/ReportDetails.cshtml", lab2);
        }
    }
}
