using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Areas.User.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
