using E_Administration.Areas.Admin.Models;
using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using User = E_Administration.Models.User;

namespace E_Administration.Controllers
{
    [Area("User")]
    public class ProfileController : Controller
    {
        private readonly DemoDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(DemoDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Profile/Details/5
        public async Task<IActionResult> Details()
        {
            var userId = GetLoggedInUserId();   // Giả định bạn có logic xác định user đã đăng nhập
            var user = await _context.Users
                        .Include(u => u.Department)
                        .FirstOrDefaultAsync(u => u.ID == userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: Profile/Edit/5
        public async Task<IActionResult> Edit()
        {
            var userId = GetLoggedInUserId();
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(User model)
        {
            var userID = GetLoggedInUserId();
            var userInDb = _context.Users.FirstOrDefault(u => u.ID == userID);

            if (userInDb != null)
            {
                try
                {
                    // Update other information
                    userInDb.UserName = model.UserName;
                    userInDb.Email = model.Email;
                    if (model.Password != null)
                    {
                        userInDb.Password = model.Password;
                    }

                    // Handle file upload
                    if (model.ImageFile != null)
                    {
                        string wwwRootPath = _webHostEnvironment.WebRootPath;
                        string uploadsFolderPath = Path.Combine(wwwRootPath, "uploads", "images");

                        // Create folder if it doesn't exist
                        if (!Directory.Exists(uploadsFolderPath))
                        {
                            Directory.CreateDirectory(uploadsFolderPath);
                        }

                        // Validate image format
                        string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
                        string extension = Path.GetExtension(model.ImageFile.FileName).ToLower();

                        if (!allowedExtensions.Contains(extension))
                        {
                            ModelState.AddModelError("ImageFile", "Only .jpg, .jpeg, and .png formats are allowed.");
                            return View(model);
                        }

                        // Delete old image if it exists
                        if (!string.IsNullOrEmpty(userInDb.Image))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath, userInDb.Image.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Save new image
                        string fileName = Path.GetFileNameWithoutExtension(model.ImageFile.FileName);
                        string uniqueFileName = fileName + "_" + DateTime.Now.ToString("yyyyMMddHHmmss") + extension;
                        string newImagePath = Path.Combine(uploadsFolderPath, uniqueFileName);

                        using (var fileStream = new FileStream(newImagePath, FileMode.Create))
                        {
                            await model.ImageFile.CopyToAsync(fileStream);
                        }

                        // Assign new image path to the user record
                        userInDb.Image = "/uploads/images/" + uniqueFileName;
                    }

                    // Save changes to the database
                    _context.Update(userInDb);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = model.ID });
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while saving changes. Please try again.");
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "User not found.");
            }

            return View(model);
        }



        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.ID == id);
        }

        private int GetLoggedInUserId()
        {
            // Giả định bạn có logic lấy ID của user hiện tại, ví dụ qua session hoặc claims
            return 2; // Ví dụ ID của user là 1 (thay thế bằng logic thực tế)
        }
    }
}
