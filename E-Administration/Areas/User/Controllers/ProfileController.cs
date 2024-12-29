using E_Administration.Areas.Admin.Models;
using E_Administration.Data;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
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
            var userID = GetLoggedInUserId(); // Get logged-in user's ID
            var userInDb = _context.Users.FirstOrDefault(u => u.ID == userID);

            if (userInDb != null)
            {
                try
                {
                    // Check if the new username already exists (excluding the current user)
                    if (_context.Users.Any(u => u.UserName == model.UserName && u.ID != userID))
                    {
                        ModelState.AddModelError("UserName", "Username already exists. Please choose a different one.");
                        return View(model);
                    }

                    // Check if the new email already exists (excluding the current user)
                    if (_context.Users.Any(u => u.Email == model.Email && u.ID != userID))
                    {
                        ModelState.AddModelError("Email", "Email already exists. Please use a different email.");
                        return View(model);
                    }

                    // Update other information
                    userInDb.UserName = model.UserName;
                    userInDb.Email = model.Email;

                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        // Check if the CurrentPassword matches the hashed password in the database
                        if (!BCrypt.Net.BCrypt.Verify(model.Password, userInDb.Password))
                        {
                            ModelState.AddModelError("Password", "Password is incorrect.");
                            return View(model);
                        }

                        // Validate confirm password
                        if (model.NewPassword == model.ConfirmPassword)
                        {
                            // Hash the new password before saving
                            userInDb.Password = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);
                        }
                        else
                        {
                            ModelState.AddModelError("ConfirmPassword", "NewPasswords and ConfirmPassword do not match.");
                            return View(model);
                        }
                    }

                    // Handle file upload (unchanged)
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
                    // Add success message
                    TempData["SuccessMessage"] = "Profile updated successfully!";
                    return RedirectToAction("Details", new { id = userInDb.ID });
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
            // Retrieve the NameIdentifier claim (UserID)
            var userIdClaim = HttpContext.User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

            // Validate and convert the claim to an integer
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("User ID not found in claims. Please ensure you are logged in.");
            }

            return userId;
        }
    }
}
