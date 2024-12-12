using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
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
                    return RedirectToAction("index");
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

                        // Xử lý file ảnh nếu được tải lên
                        if (model.ImageFile != null)
                        {
                            var fileName = Path.GetFileName(model.ImageFile.FileName);
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/img", fileName);

                            // Lưu file vào thư mục wwwroot/img
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                model.ImageFile.CopyTo(stream);
                            }

                            // Cập nhật đường dẫn ảnh
                            aboutUs.ImageUrl = "/img/" + fileName;
                        }

                        ctx.SaveChanges();
                    }

                    return RedirectToAction("Index");
                }

                return View(model);
            }

        
    }
}
