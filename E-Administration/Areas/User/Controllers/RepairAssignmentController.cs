using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Areas.User.Controllers
{
    [Area("User")]
    [Route("User/[controller]")]
    public class RepairAssignmentController : Controller
    {
        private readonly DemoDbContext _dbContext;

        public RepairAssignmentController(DemoDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("Index")]
        public IActionResult Index()
        {
            var assignments = _dbContext.RepairAssignments
<<<<<<< HEAD
                .Include(ra => ra.IssueReports) // Sử dụng navigation property đúng
                .Include(ra => ra.Technician) // Sử dụng navigation property đúng
                .ToList();

            return View(assignments);
=======
        .Include(ra => ra.IssueReports)
        .Include(ra => ra.Technician)
        .ToList();

            return View(assignments);

>>>>>>> hhuy
        }

        [HttpGet("Create")]
        public IActionResult Create()
        {
            // Lấy dữ liệu từ cơ sở dữ liệu
            var issueReports = _dbContext.IssueReports.ToList();
            var users = _dbContext.Users
       .Where(user => user.Role == "Technician")
       .ToList();



            // Kiểm tra dữ liệu
            if (issueReports == null || !issueReports.Any())
            {
                TempData["ErrorMessage"] = "No issue reports available.";
            }

            if (users == null || !users.Any())
            {
                TempData["ErrorMessage"] = "No users available.";
            }

            // Gán vào ViewBag để truyền đến View
            ViewBag.IssueReports = issueReports;
            ViewBag.Users = users;



            return View();
        }

        [HttpPost("Create")]
        public IActionResult Create(RepairAssignments assignment)
        {
            if (ModelState.IsValid)
            {
                assignment.CreatedAt = DateTime.Now; // Gán thời gian hiện tại

                _dbContext.RepairAssignments.Add(assignment);
                _dbContext.SaveChanges();
                return RedirectToAction("Index");
            }

            // Nếu ModelState không hợp lệ, load lại dữ liệu cho ViewBag
            ViewBag.IssueReports = _dbContext.IssueReports.ToList();

            ViewBag.Users = _dbContext.Users
       .Where(user => user.Role == "Technician")
       .ToList();

            return View(assignment);
        }

<<<<<<< HEAD
        //[HttpGet("Edit/{id}")]
        //public IActionResult Edit(int id)
        //{
        //    var assignment = _dbContext.RepairAssignments
        //        .Include(ra => ra.IssueReports)
        //        .Include(ra => ra.Technician)
        //        .FirstOrDefault(ra => ra.ID == id);

        //    if (assignment == null)
        //        return NotFound();

        //    ViewBag.IssueReports = _dbContext.IssueReports.ToList();
        //    ViewBag.Technicians = _dbContext.Users.ToList();
        //    return View(assignment);
        //}

        //[HttpPost("Edit/{id}")]
        //public IActionResult Edit(RepairAssignments assignment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        assignment.UpdatedAt = DateTime.Now;
        //        _dbContext.RepairAssignments.Update(assignment);
        //        _dbContext.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    ViewBag.IssueReports = _dbContext.IssueReports.ToList();
        //    ViewBag.Technicians = _dbContext.Users.ToList();
        //    return View(assignment);
        //}

=======
>>>>>>> hhuy

        [HttpGet("Details/{id}")]
        public IActionResult Details(int id)
        {
            var assignment = _dbContext.RepairAssignments
                .Include(ra => ra.IssueReports) // Bao gồm thông tin báo cáo sự cố
                .Include(ra => ra.Technician) // Bao gồm thông tin kỹ thuật viên
                .FirstOrDefault(ra => ra.ID == id);

            if (assignment == null)
            {
                TempData["ErrorMessage"] = "Repair Assignment not found.";
                return RedirectToAction("Index");
            }

            return View(assignment);
        }


<<<<<<< HEAD
        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var assignment = _dbContext.RepairAssignments.Find(id);
            if (assignment == null) return NotFound();

            _dbContext.RepairAssignments.Remove(assignment);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
=======

        [HttpGet("Confirm/{id}")] // Rõ ràng endpoint cho Confirm GET
        public IActionResult Confirm(int id)
        {
            var assignment = _dbContext.RepairAssignments
                .Include(a => a.Technician)
                .Include(a => a.IssueReports)
                .FirstOrDefault(a => a.ID == id);

            if (assignment == null)
            {
                return NotFound();
            }

            return View("Confirm", assignment);
        }

        [HttpPost("Confirm/{id}")] // Rõ ràng endpoint cho Confirm POST
        public IActionResult ConfirmConfirmed(int id)
        {
            var assignment = _dbContext.RepairAssignments.Find(id);

            if (assignment == null)
            {
                return NotFound();
            }

            assignment.IsConfirmed = true;
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Repair assignment has been successfully confirmed.";

            return RedirectToAction("Index");
        }

        [HttpGet("Delete/{id}")] // Rõ ràng endpoint cho Delete GET
        public IActionResult Delete(int id)
        {
            var assignment = _dbContext.RepairAssignments
                .Include(a => a.Technician)
                .Include(a => a.IssueReports)
                .FirstOrDefault(a => a.ID == id);

            if (assignment == null)
            {
                return NotFound();
            }

            return View("Delete", assignment);
        }

        [HttpPost("Delete/{id}")] // Rõ ràng endpoint cho Delete POST
        public IActionResult DeleteConfirmed(int id)
        {
            var assignment = _dbContext.RepairAssignments.Find(id);

            if (assignment == null)
            {
                return NotFound();
            }

            _dbContext.RepairAssignments.Remove(assignment);
            _dbContext.SaveChanges();

            TempData["SuccessMessage"] = "Repair assignment deleted successfully.";

            return RedirectToAction("Index");
        }

        [HttpGet("DetailsRepair/{id}")]
        public IActionResult DetailRepair(int id)
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



>>>>>>> hhuy
    }
}
