using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Controllers
{
    public class HomeController : Controller
    {
        private readonly DemoDbContext _context;

        public HomeController(DemoDbContext ctx)
        {
            _context = ctx;
        }
        public IActionResult Index()
        {

            // Lấy danh sách eLearning từ database
            var ELearns = _context.ELearning.ToList();

            // Tạo ViewModel và gán danh sách Elearnings
            var viewModel = new HomeViewModel
            {
                Elearnings = ELearns
            };
            return View(viewModel);
        }
    }
}
