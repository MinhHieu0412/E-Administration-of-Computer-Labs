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
                Notes = assignment.Notes
            };

            await ctx.Assignments.AddAsync(newAssignment);
            await ctx.SaveChangesAsync();

            return Json(new { success = true, message = "Assignment created successfully." });
        }



    }


}
