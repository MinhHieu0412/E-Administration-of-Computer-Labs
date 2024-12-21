using E_Administration.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Administration.Areas.User.Controllers
{
    [Area("User")]
    public class AssignmentController : Controller
    {
        private readonly DemoDbContext ctx;

        public AssignmentController(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy UserId từ Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); // Nếu không có thông tin người dùng
            }

            /*foreach (var claim in User.Claims)
            {
                Console.WriteLine($"Type: {claim.Type}, Value: {claim.Value}");
            }*/

            int userId = int.Parse(userIdClaim.Value);

            // Lấy danh sách Assignment của người dùng
            var assignments = await ctx.Assignments
                .Where(a => a.UserID == userId)
                .ToListAsync();

            /*var assignment = await ctx.Assignments.ToListAsync();*/

            return View(assignments);
        }
    }
}
