using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Controllers
{
<<<<<<<< HEAD:E-Administration/Controllers/HomeController.cs
    public class HomeController : Controller
========
    [Area("User")]
    public class PageUserController : Controller
>>>>>>>> duyanh:E-Administration/Areas/User/Controllers/PageUserController.cs
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
