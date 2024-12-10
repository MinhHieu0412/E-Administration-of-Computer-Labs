using E_Administration.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LecturerController : Controller
    {
        private readonly DemoDbContext ctx;

        public LecturerController(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var lecturers = await ctx.Users.Where(x=>x.Role == "lecturer").ToListAsync();
            return View(lecturers);
        }
    }
}
