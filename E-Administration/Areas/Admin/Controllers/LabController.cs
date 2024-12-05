using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Areas.Admin.Controllers
{
    public class LabController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
