using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Controllers
{
    public class HomeController : Controller
    {
        private readonly DemoDbContext ctx;

        public HomeController(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }

        public async Task<IActionResult> Index()
        {
            var ELearnings = await ctx.ELearning.ToListAsync();
            var lab = await ctx.Labs.ToListAsync();

            // Tạo ViewModel và gán danh sách Elearnings
            var viewModel = new HomeViewModel
            {
                Elearnings = ELearnings,
                Labs = lab
            };
            return View(viewModel);
        }

        public async Task<IActionResult> DetailLab(int id)
        {
            var lab = await ctx.Labs
               .Include(x => x.Equipments) // Eager Loading: Lấy luôn dữ liệu từ bảng Equipment liên quan
               .SingleOrDefaultAsync(x => x.ID == id);
            if (lab == null)
            {
                return NotFound();
            }
            return View(lab);
        }

        [HttpGet]
        public async Task<IActionResult> DisplayLab(int id)
        {
            var labs = await ctx.Labs.ToListAsync();
            return View(labs);
        }

    }
    }
