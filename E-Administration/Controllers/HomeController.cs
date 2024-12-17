using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
