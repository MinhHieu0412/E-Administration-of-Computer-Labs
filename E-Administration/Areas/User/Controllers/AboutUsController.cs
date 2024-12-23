    using E_Administration.Data;
    using E_Administration.Models;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;

    namespace E_Administration.Areas.User.Controllers
    {
        [Area("User")]
        public class AboutUsController : Controller
        {
            private readonly DemoDbContext ctx;

            public AboutUsController(DemoDbContext context)
            {
                ctx = context;
            }

            public IActionResult Index()
            {
                var aboutUs = ctx.AboutUs.FirstOrDefault();
                return View(aboutUs);
            }

            public IActionResult Edit()
            {
                var aboutUs = ctx.AboutUs.FirstOrDefault();
                if (aboutUs == null)
                {
                    return RedirectToAction("Create");
                }

                return View(aboutUs);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Edit(AboutUs model)
            {
                if (ModelState.IsValid)
                {
                    var aboutUs = ctx.AboutUs.FirstOrDefault();
                    if (aboutUs != null)
                    {
                        aboutUs.Description = model.Description;
                        aboutUs.Mission = model.Mission;
                        aboutUs.ImageUrl = model.ImageUrl; // Save the image URL
                        ctx.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }

                return View(model);
            }

            public IActionResult Create()
            {
                return View();
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public IActionResult Create(AboutUs model)
            {
                if (ModelState.IsValid)
                {
                    ctx.AboutUs.Add(model);
                    ctx.SaveChanges();
                    return RedirectToAction("Index");
                }

                return View(model);
            }
        }
    }
