using E_Administration.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Controllers
{
    public class ELearningController : Controller
    {
        private readonly DemoDbContext _context;

        public ELearningController(DemoDbContext ctx)
        {
            _context = ctx;
        }
        public IActionResult Index()
        {
            var elearn = _context.ELearning.ToList();
            
            return View(elearn);
        }
        public IActionResult ViewElearning()
        {
            var elearn = _context.ELearning.Include(e => e.User).ToList();

            return View(elearn);
        }
    }
}
