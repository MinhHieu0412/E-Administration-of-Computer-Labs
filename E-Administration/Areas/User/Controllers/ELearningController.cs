using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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
            // Retrieve the logged-in user's ID from claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(); // Handle missing or invalid claims
            }

            // Query Elearnings for the logged-in user
            var userElearnings = await ctx.ELearning
                                          .Where(e => e.UploadedBy == userId) // No need for conversion here
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

            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
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

            // Validate Title inputs
            if (eLearning.Title.Trim() == null )
            {
                ModelState.AddModelError("Title", "Title is required.");
                return View(eLearning);
            }

            if (eLearning.Description.Trim() == null)
            {
                ModelState.AddModelError("Description", "Description is required.");
                return View(eLearning);
            }
            // Validate file inputs
            if (file == null || file.Length == 0)
            {
                    ModelState.AddModelError("FilePath", "PDF file is required.");
                                    return View(eLearning);
            }
                

            if (image == null || image.Length == 0)
            {
                ModelState.AddModelError("Link", "Image is required.");
                return View(eLearning);
            }
                

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

            var eLearning = await ctx.ELearning.FindAsync(id);
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
                // Kiểm tra định dạng file PDF
                if (file != null && file.Length > 0)
                {
                    string[] allowedPdfExtensions = { ".pdf" };
                    string fileExtension = Path.GetExtension(file.FileName).ToLower();

                    if (!allowedPdfExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("FilePath", "Invalid file format. Only PDF files are allowed.");
                    }
                    else
                    {
                        try
                        {
                            string pdfFolder = Path.Combine(env.WebRootPath, "uploads/pdf");
                            string pdfFileName = Guid.NewGuid() + fileExtension;
                            string pdfFilePath = Path.Combine(pdfFolder, pdfFileName);

                            if (!Directory.Exists(pdfFolder))
                                Directory.CreateDirectory(pdfFolder);

                            using (var stream = new FileStream(pdfFilePath, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }

                            // Xóa file PDF cũ
                            string oldPdfPath = Path.Combine(env.WebRootPath, existingELearning.FilePath.TrimStart('/'));
                            if (System.IO.File.Exists(oldPdfPath))
                            {
                                System.IO.File.Delete(oldPdfPath);
                            }

                            existingELearning.FilePath = $"/uploads/pdf/{pdfFileName}";
                        }
                        catch (IOException ex)
                        {
                            ModelState.AddModelError("FilePath", $"Error while saving the PDF file: {ex.Message}");
                        }
                    }
                }

                // Kiểm tra định dạng hình ảnh
                if (image != null && image.Length > 0)
                {
                    string[] allowedImageExtensions = { ".jpg", ".jpeg", ".png", ".gif" };
                    string imageExtension = Path.GetExtension(image.FileName).ToLower();

                    if (!allowedImageExtensions.Contains(imageExtension))
                    {
                        ModelState.AddModelError("Link", "Invalid image format. Only JPG, JPEG, PNG, or GIF files are allowed.");
                    }
                    else
                    {
                        try
                        {
                            string imageFolder = Path.Combine(env.WebRootPath, "uploads/images");
                            string imageFileName = Guid.NewGuid() + imageExtension;
                            string imageFilePath = Path.Combine(imageFolder, imageFileName);

                            if (!Directory.Exists(imageFolder))
                                Directory.CreateDirectory(imageFolder);

                            using (var stream = new FileStream(imageFilePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            // Xóa hình ảnh cũ
                            string oldImagePath = Path.Combine(env.WebRootPath, existingELearning.Link.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }

                            existingELearning.Link = $"/uploads/images/{imageFileName}";
                        }
                        catch (IOException ex)
                        {
                            ModelState.AddModelError("Link", $"Error while saving the image file: {ex.Message}");
                        }
                    }
                }

                /*// Kiểm tra lại trạng thái ModelState
                if (!ModelState.IsValid)
                {
                    return View(existingELearning); // Trả lại view với thông tin lỗi
                }*/

                // Cập nhật các trường khác
                existingELearning.Title = eLearning.Title;
                existingELearning.Description = eLearning.Description;
                existingELearning.UpdatedAt = DateTime.UtcNow;

                await ctx.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An unexpected error occurred: {ex.Message}");
                return View(existingELearning);
            }
        }


        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var eLearning = await ctx.ELearning
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

        public IActionResult Details(int id)
        {
            var elearning = ctx.ELearning.Include(e => e.User).FirstOrDefault(e => e.ID == id);
            if (elearning == null)
            {
                return NotFound();
            }
            return View(elearning);
        }
    }

}
