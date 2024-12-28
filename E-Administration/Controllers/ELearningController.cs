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

        // Display details of a specific e-learning resource
        public IActionResult Details(int id)
        {
            var elearning = _context.ELearning.Include(e => e.User).FirstOrDefault(e => e.ID == id);
            if (elearning == null)
            {
                return NotFound();
            }
            return View(elearning);
        }

    }
}
