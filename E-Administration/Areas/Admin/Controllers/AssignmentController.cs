using E_Administration.Data;
using E_Administration.Dto;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AssignmentController : Controller
    {
        private readonly DemoDbContext ctx;

        public AssignmentController(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var assignments = await ctx.Assignments
                .Include(a => a.User)  // Tham chiếu đến bảng User
                .Include(a => a.Lab)   // Tham chiếu đến bảng Lab
                .ToListAsync();

            // Check if assignments are completed based on due date
            foreach (var assignment in assignments)
            {
                // Assuming Date + TimeStart or another DateTime property represents the due date
                var dueDate = assignment.Date.Add(assignment.TimeStart); // Adjust this according to your logic

                if (dueDate < DateTime.Now)
                {
                    assignment.Status = "Hoàn thành"; // Mark as completed if past due date
                }
                else
                {
                    assignment.Status = "Chưa hoàn thành"; // Mark as incomplete if not passed
                }
            }

            var users = await ctx.Users.Where(u=>u.Role == "Lecturer").ToListAsync();
            var labs = await ctx.Labs.ToListAsync();

            ViewBag.Users = users;
            ViewBag.Labs = labs;

            return View(assignments);
        }


        [HttpPost]
        public async Task<IActionResult> CreateAssignment([FromBody] AssignmentDto assignment)
        {
            var newAssignment = new Assignments
            {
                UserID = assignment.UserID,
                LabID = assignment.LabID,
                Date = assignment.Date,
                TimeStart = assignment.TimeStart,
                TimeEnd = assignment.TimeEnd,
                Notes = assignment.Notes,
                Status = "UnCompleted",
            };

            await ctx.Assignments.AddAsync(newAssignment);
            await ctx.SaveChangesAsync();

            return Json(new { success = true, message = "Assignment created successfully." });
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAjax(int id)
        {
            var assignment = await ctx.Assignments
                .Include(a => a.User)
                .Include(a => a.Lab)
                .FirstOrDefaultAsync(a => a.ID == id);

            if (assignment != null)
            {
                // Danh sách người dùng và phòng lab
                var users = await ctx.Users
                    .Where(u => u.Role == "Lecturer") // Chỉ lấy những người dùng có vai trò "Lecturer"
                    .Select(u => new
                    {
                        u.ID,
                        u.UserName
                    })
                    .ToListAsync();

                var labs = await ctx.Labs
                    .Select(l => new
                    {
                        l.ID,
                        l.Name
                    })
                    .ToListAsync();

                // Dữ liệu bài phân công
                var assignmentData = new
                {
                    assignment.ID,
                    assignment.UserID,
                    assignment.LabID,
                    assignment.Date,
                    assignment.TimeStart,
                    assignment.TimeEnd,
                    assignment.Notes,
                    assignment.Status,
                    users, // Thêm danh sách người dùng
                    labs   // Thêm danh sách phòng lab
                };

                return Json(assignmentData);
            }

            return Json(new { success = false, message = "Assignment not found." });
        }


        // Handle assignment editing (This should be linked to the AJAX call for submitting the edited assignment)
        [HttpPost]
        public async Task<IActionResult> EditAssignment([FromBody] AssignmentDto model)
        {
            if (ModelState.IsValid)
            {
                var assignment = await ctx.Assignments.FindAsync(model.ID);
                if (assignment != null)
                {
                    assignment.UserID = model.UserID;
                    assignment.LabID = model.LabID;
                    assignment.Date = model.Date;
                    assignment.TimeStart = model.TimeStart;
                    assignment.TimeEnd = model.TimeEnd;
                    assignment.Notes = model.Notes;
                    assignment.Status = model.Status;
                    await ctx.SaveChangesAsync();
                    return Json(new { success = true, message = "Assignment updated successfully." });
                }
            }
            return Json(new { success = false, message = "Failed to update assignment." });

        }


        public async Task<IActionResult> Delete(int id)
        {
            var assignment = await ctx.Assignments.FindAsync(id);

            if (assignment != null)
            {
                ctx.Assignments.Remove(assignment);
                await ctx.SaveChangesAsync();

                TempData["ToastrMessage"] = "Assignment deleted successfully.";
                TempData["ToastrType"] = "success"; // Bạn có thể đặt "info", "warning", "error" tùy theo thông báo.
            }
            else
            {
                TempData["ToastrMessage"] = "Assignment not found.";
                TempData["ToastrType"] = "error";
            }

            return RedirectToAction("Index");
        }



    }


}
