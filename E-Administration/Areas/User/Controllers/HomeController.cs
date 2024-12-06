using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Areas.User.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
