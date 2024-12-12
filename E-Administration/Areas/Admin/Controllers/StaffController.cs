using E_Administration.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StaffController : Controller
    {
        private readonly DemoDbContext ctx;

        public StaffController(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var staffs = await ctx.Users.Where(x=>x.Role == "staff").ToListAsync();
            return View(staffs);
        }
    }
}
