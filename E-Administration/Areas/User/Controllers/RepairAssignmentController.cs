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
                .Include(ra => ra.IssueReports) // Sử dụng navigation property đúng
                .Include(ra => ra.Technician) // Sử dụng navigation property đúng
                .ToList();

            return View(assignments);
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


        [HttpPost("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            var assignment = _dbContext.RepairAssignments.Find(id);
            if (assignment == null) return NotFound();

            _dbContext.RepairAssignments.Remove(assignment);
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
