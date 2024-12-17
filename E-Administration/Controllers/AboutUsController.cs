using E_Administration.Data;
using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Controllers
{
    public class AboutUsController : Controller
    {
        private readonly DemoDbContext ctx;

        public AboutUsController(DemoDbContext context)
        {
            ctx = context;
        }

        public IActionResult AboutUs()
        {
            var aboutUs = ctx.AboutUs.FirstOrDefault();
            return View(aboutUs);
        }
    }
}
