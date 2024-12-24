using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace E_Administration.Areas.User.Controllers
{
    [Area("User")]
    public class PageUserController : Controller
    {
        private readonly DemoDbContext _context;

        public PageUserController(DemoDbContext ctx)
        {
            _context = ctx;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách eLearning từ database
            var ELearns = await _context.ELearning.ToListAsync();
            var lab = await _context.Labs.ToListAsync();

            // Tạo ViewModel và gán danh sách Elearnings
            var viewModel = new HomeViewModel
            {
                Elearnings = ELearns,
                Labs =  lab
            };
           

            // Truyền ViewModel vào View
            return View(viewModel);
        }

        public async Task<IActionResult> DisplayAssignment()
        {
            // Lấy UserId từ Claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null)
            {
                return Unauthorized(); // Nếu không có thông tin người dùng
            }

            int userId = int.Parse(userIdClaim.Value);

            // Lấy danh sách Assignment của người dùng
            var assignments = await _context.Assignments
                .Where(a => a.UserID == userId)
                .ToListAsync();

            return View(assignments);
        }


        public async Task<IActionResult> DetailLab(int id)
        {
            var lab = await _context.Labs
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
            var labs = await _context.Labs.ToListAsync();
            return View(labs);
        }



    }
}
