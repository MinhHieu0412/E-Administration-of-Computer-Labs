using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Areas.User.Controllers
{
    [Area("User")]
    public class ELearningController : Controller
    {
        private readonly DemoDbContext ctx;
        private readonly IWebHostEnvironment env;

        public ELearningController(DemoDbContext context, IWebHostEnvironment environment)
        {
            ctx = context;
            env = environment;
        }
        public async Task<IActionResult> Index()
        {
            return View(await ctx.ELearnings.ToListAsync());
        }

        public IActionResult Create()
        {
            return View();
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ELearning eLearning, IFormFile file, IFormFile image)
        {
            if (file == null || file.Length == 0)
                ModelState.AddModelError("FilePath", "File PDF is required.");

            if (image == null || image.Length == 0)
                ModelState.AddModelError("Link", "Image is required.");

            if (ModelState.IsValid)
            {
                // Tải tệp PDF
                string pdfFolder = Path.Combine(env.WebRootPath, "uploads/pdf");
                if (!Directory.Exists(pdfFolder))
                    Directory.CreateDirectory(pdfFolder);

                string pdfFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string pdfFilePath = Path.Combine(pdfFolder, pdfFileName);
                using (var stream = new FileStream(pdfFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Tải hình ảnh demo
                string imageFolder = Path.Combine(env.WebRootPath, "uploads/images");
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                string imageFileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                string imageFilePath = Path.Combine(imageFolder, imageFileName);
                using (var stream = new FileStream(imageFilePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Lưu thông tin vào database
                eLearning.FilePath = $"/uploads/pdf/{pdfFileName}";
                eLearning.Link = $"/uploads/images/{imageFileName}";
                eLearning.CreatedAt = DateTime.Now;
                eLearning.UpdatedAt = DateTime.Now;

                ctx.Add(eLearning);
                await ctx.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(eLearning);
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eLearning = await ctx.ELearnings.FindAsync(id);
            if (eLearning == null)
            {
                return NotFound();
            }

            return View(eLearning);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ELearning eLearning, IFormFile? file, IFormFile? image)
        {
            if (id != eLearning.Id)
            {
                return NotFound();
            }

            var existingELearning = await ctx.ELearnings.FindAsync(id);
            if (existingELearning == null)
            {
                return NotFound();
            }

            try
            {
                // Xử lý file PDF
                if (file == null || file.Length == 0)
                {
                    // Không thay đổi file PDF
                    eLearning.FilePath = existingELearning.FilePath;
                }
                else
                {
                    // Xóa file PDF cũ
                    string oldPdfPath = Path.Combine(env.WebRootPath, existingELearning.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldPdfPath))
                    {
                        System.IO.File.Delete(oldPdfPath);
                    }

                    // Tải file PDF mới
                    string pdfFolder = Path.Combine(env.WebRootPath, "uploads/pdf");
                    string pdfFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                    string pdfFilePath = Path.Combine(pdfFolder, pdfFileName);

                    if (!Directory.Exists(pdfFolder))
                        Directory.CreateDirectory(pdfFolder);

                    using (var stream = new FileStream(pdfFilePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                    eLearning.FilePath = $"/uploads/pdf/{pdfFileName}";
                }

                // Xử lý image
                if (image == null || image.Length == 0)
                {
                    // Không thay đổi image
                    eLearning.Link = existingELearning.Link;
                }
                else
                {
                    // Xóa hình ảnh cũ
                    string oldImagePath = Path.Combine(env.WebRootPath, existingELearning.Link.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }

                    // Tải hình ảnh mới
                    string imageFolder = Path.Combine(env.WebRootPath, "uploads/images");
                    string imageFileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                    string imageFilePath = Path.Combine(imageFolder, imageFileName);

                    if (!Directory.Exists(imageFolder))
                        Directory.CreateDirectory(imageFolder);

                    using (var stream = new FileStream(imageFilePath, FileMode.Create))
                    {
                        await image.CopyToAsync(stream);
                    }
                    eLearning.Link = $"/uploads/images/{imageFileName}";
                }

                // Cập nhật các trường khác
                eLearning.CreatedAt = existingELearning.CreatedAt;
                eLearning.UpdatedAt = DateTime.Now;

                ctx.Entry(existingELearning).CurrentValues.SetValues(eLearning);
                await ctx.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Có lỗi xảy ra: {ex.Message}");
                return View(eLearning);
            }
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eLearning = await ctx.ELearnings
                .FirstOrDefaultAsync(m => m.Id == id);
            if (eLearning == null)
            {
                return NotFound();
            }

            return View(eLearning);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var eLearning = await ctx.ELearnings.FindAsync(id);
            if (eLearning != null)
            {
                string pdfPath = Path.Combine(env.WebRootPath, eLearning.FilePath.TrimStart('/'));
                string imagePath = Path.Combine(env.WebRootPath, eLearning.Link.TrimStart('/'));

                if (System.IO.File.Exists(pdfPath))
                {
                    System.IO.File.Delete(pdfPath);
                }

                if (System.IO.File.Exists(imagePath))
                {
                    System.IO.File.Delete(imagePath);
                }

                ctx.ELearnings.Remove(eLearning);
                await ctx.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ELearningExists(int id)
        {
            return ctx.ELearnings.Any(e => e.Id == id);
        }
    }

}
