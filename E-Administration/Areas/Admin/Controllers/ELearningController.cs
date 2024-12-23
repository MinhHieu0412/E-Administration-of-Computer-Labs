﻿using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
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

            // Query Elearnings for the logged-in user
            var userElearnings = await ctx.ELearning
                                           .Include(e => e.User)
                                           .ToListAsync();

            return View(userElearnings);
        }


        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Elearnings eLearning, IFormFile file, IFormFile image)
        {
            ViewBag.DebugElearning = eLearning;

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim))
            {
                ModelState.AddModelError(string.Empty, "User ID not found. Please ensure you are logged in.");
                return View(eLearning);
            }

            int userId;
            try
            {
                userId = int.Parse(userIdClaim);
            }
            catch (FormatException)
            {
                ModelState.AddModelError(string.Empty, "Invalid User ID format.");
                return View(eLearning);
            }

            // Validate file inputs
            if (file == null || file.Length == 0)
                ModelState.AddModelError("FilePath", "PDF file is required.");

            if (image == null || image.Length == 0)
                ModelState.AddModelError("Link", "Image is required.");

            /*if (!ModelState.IsValid)
                return View(eLearning);*/

            try
            {
                // Save PDF file
                string pdfFolder = Path.Combine(env.WebRootPath, "uploads/pdf");
                if (!Directory.Exists(pdfFolder))
                    Directory.CreateDirectory(pdfFolder);

                string pdfFileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
                string pdfFilePath = Path.Combine(pdfFolder, pdfFileName);
                using (var stream = new FileStream(pdfFilePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Save image file
                string imageFolder = Path.Combine(env.WebRootPath, "uploads/images");
                if (!Directory.Exists(imageFolder))
                    Directory.CreateDirectory(imageFolder);

                string imageFileName = Guid.NewGuid() + Path.GetExtension(image.FileName);
                string imageFilePath = Path.Combine(imageFolder, imageFileName);
                using (var stream = new FileStream(imageFilePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Create new Elearnings object
                var elearningToAdd = new Elearnings
                {
                    UploadedBy = userId, // Assign the user ID from claims
                    Title = eLearning.Title,
                    Description = eLearning.Description,
                    FilePath = $"/uploads/pdf/{pdfFileName}",
                    Link = $"/uploads/images/{imageFileName}"

                };

                // Save to database
                ctx.ELearning.Add(elearningToAdd);
                await ctx.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException dbEx)
            {
                ModelState.AddModelError(string.Empty, "An error occurred while saving the resource: " + dbEx.InnerException?.Message);
                return View(eLearning);
            }
        }



        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eLearning = await ctx.ELearning
                                       .Include(e => e.User)
                                       .FirstOrDefaultAsync(e => e.ID == id);
            if (eLearning == null)
            {
                return NotFound();
            }

            return View(eLearning);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Elearnings eLearning, IFormFile? file, IFormFile? image)
        {
            if (id != eLearning.ID)
            {
                return NotFound();
            }

            var existingELearning = await ctx.ELearning.FindAsync(id);
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
                eLearning.UpdatedAt = DateTime.UtcNow; // Thực hiện cập nhật thủ công

                // Cập nhật giá trị của đối tượng hiện tại với đối tượng eLearning
                ctx.Entry(existingELearning).CurrentValues.SetValues(eLearning);

                // Đảm bảo rằng EF nhận ra sự thay đổi của UpdatedAt
                ctx.Entry(existingELearning).Property(e => e.UpdatedAt).IsModified = true;

                await ctx.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(eLearning);
            }
        }



        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Include the User data
            var eLearning = await ctx.ELearning
                                      .Include(e => e.User) // Include the uploader's data
                                      .FirstOrDefaultAsync(m => m.ID == id);

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
            var eLearning = await ctx.ELearning.FindAsync(id);
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

                ctx.ELearning.Remove(eLearning);
                await ctx.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool ELearningExists(int id)
        {
            return ctx.ELearning.Any(e => e.ID == id);
        }
    }
}
