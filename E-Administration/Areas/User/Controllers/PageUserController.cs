using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Areas.User.Controllers
{
    [Area("User")]
    public class PageUserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
