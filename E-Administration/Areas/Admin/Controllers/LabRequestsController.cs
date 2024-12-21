using E_Administration.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LabRequestsController : Controller
    {
        
        private readonly DemoDbContext _context;

        public LabRequestsController(DemoDbContext context)
        {
            _context = context;
        }

        // GET: View Lab Requests
        public IActionResult Index()
        {
            var labRequests = _context.LabRequests
        .Include(l => l.Department) // Eager load Department
        .ToList();
            return View(labRequests);
        }

        // POST: Update Status and Response
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult UpdateStatus(int id, string status, string adminResponse)
        {
            var labRequest = _context.LabRequests.FirstOrDefault(l => l.ID == id);

            if (labRequest == null)
            {
                return NotFound();
            }

            labRequest.Status = status;
            labRequest.AdminResponse = adminResponse;
            labRequest.UpdatedAt = DateTime.Now; // Update timestamp

            _context.Update(labRequest);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}
