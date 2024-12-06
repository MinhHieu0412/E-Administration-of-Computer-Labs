using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class LabController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
