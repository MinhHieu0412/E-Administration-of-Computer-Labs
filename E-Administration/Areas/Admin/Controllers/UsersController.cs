
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using E_Administration.Data;
using Microsoft.EntityFrameworkCore;
using System.Net.Mail;
using System.Net;
using MimeKit;
using MailKit.Net.Smtp;

namespace E_Administration.Controllers
{
    [Area("Admin")]
    public class UsersController : Controller
    {
        private readonly DemoDbContext _context;

        public UsersController(DemoDbContext ctx)
        {
            _context = ctx;
        }

        // List Users
        public ActionResult Index(string searchString = null, string roleFilter = null)
        {
            // Get the list of roles to display in the dropdown
            ViewBag.Roles = _context.Users
                .Select(u => u.Role)
                .Distinct()
                .ToList();

            // Save roleFilter value to ViewBag to retain selection in dropdown
            ViewBag.RoleFilter = roleFilter;

            // Eager load Department bằng Include
            var users = _context.Users.Include(u => u.Department).AsQueryable();

            
            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.Role == roleFilter);
            }

            // If there is a searchString, filter by username or email
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.UserName.Contains(searchString) || u.Email.Contains(searchString));
            }

            // Save searchString value to ViewData to retain in search form
            ViewData["SearchString"] = searchString;

            // Returns the list of users after filtering
            return View(users.ToList());
        }




        // Create User (GET)
        public ActionResult Create()
        {
            // Pass the Departments and Roles list to the View
            ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name");
            ViewBag.Roles = new SelectList(new List<string> { "Student", "Lecturer", "HOD", "Technician" });
            return View();
        }

        // Create User (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(User user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already exists. Please use another email.");
                ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name");
                ViewBag.Roles = new SelectList(new List<string> { "Student", "Lecturer", "HOD", "Technician" });
                return View(user);
            }

            if (_context.Users.Any(u => u.UserName == user.UserName))
            {
                ModelState.AddModelError("UserName", "Username already exists. Please choose another one.");
                ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name");
                ViewBag.Roles = new SelectList(new List<string> { "Student", "Lecturer", "HOD", "Technician" });
                return View(user);
            }

            try
            {
                // Generate random password
                string randomPassword = GenerateRandomPassword();
                user.Password = BCrypt.Net.BCrypt.HashPassword(randomPassword);
                user.Status = true;

                _context.Users.Add(user);
                _context.SaveChanges();

                // Attempt to send the email
                try
                {
                    SendAccountEmail(user.Email, user.UserName, randomPassword);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "User was created successfully, but failed to send email: " + ex.Message;
                    return RedirectToAction("Index");
                }

                TempData["SuccessMessage"] = "User was created successfully and email was sent.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while creating the user: " + ex.Message);
                ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name");
                ViewBag.Roles = new SelectList(new List<string> { "Student", "Lecturer", "HOD", "Technician" });
                return View(user);
            }
        }


        [HttpGet("Details/{id}")]
        public IActionResult Details(int id)
        {
            var user = _context.Users.FirstOrDefault(u => u.ID == id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Index", "RepairAssignment");
            }

            return View(user);
        }
        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void SendAccountEmail(string email, string userName, string password)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("E_Administration", "caonguyen2288@gmail.com")); 
            message.To.Add(new MailboxAddress(userName, email));
            message.Subject = "Welcome to E_Administration - Your Account Details";

            
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
            <h3>Hello {userName},</h3>
            <p>Your account has been successfully created.</p>
            <p><b>Username:</b> {userName}</p>
            <p><b>Email:</b> {email}</p>
            <p><b>Password:</b> {password}</p>
            <p>Please log in and change your password immediately: 
               <a href='http://localhost:5285/Account/Login'>Login Here</a></p> 
            <p>Thank you,</p>
            <p><b>E_Administration Team</b></p>
        ",
                TextBody = $@"
            Hello {userName},

            Your account has been successfully created.

            Username: {userName}
            Email:    {email}
            Password: {password}

            Please log in and change your password immediately:
            http://localhost:5285/Account/Login

            Thank you,
            E_Administration Team
        "
            };

            message.Body = bodyBuilder.ToMessageBody();

            using (var client = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    // Connect to SMTP server
                    client.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);

                    // Authenticate
                    client.Authenticate("caonguyen2288@gmail.com", "jcxf ntvr buwh ibne"); 

                    // Send the email
                    client.Send(message);
                }
                catch (Exception ex)
                {
                    throw new Exception("Failed to send email: " + ex.Message, ex);
                }
                finally
                {
                    client.Disconnect(true);
                }
            }
        }



        // Edit User (GET)
        public ActionResult Edit(int id)
        {
            var user = _context.Users.Find(id);
            if (user == null) return NotFound();

            ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name", user.DepartmentID);
            ViewBag.Roles = new SelectList(new[] { "Student", "Lecturer", "HOD", "Technician" }, user.Role);
            return View(user);
        }

        // Edit User (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            try
            {
                
                var existingUser = _context.Users.Find(user.ID);
                if (existingUser == null)
                {
                    ModelState.AddModelError("", "User not found.");
                    ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name", user.DepartmentID);
                    ViewBag.Roles = new SelectList(new[] { "Student", "Lecturer", "HOD", "Technician" }, user.Role);
                    return View(user);
                }

                // Check if email has changed and duplicates
                if (!string.Equals(existingUser.Email, user.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var emailExists = _context.Users.Any(u => u.Email == user.Email && u.ID != user.ID);
                    if (emailExists)
                    {
                        ModelState.AddModelError("Email", "Email is already in use by another user.");
                        ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name", user.DepartmentID);
                        ViewBag.Roles = new SelectList(new[] { "Student", "Lecturer", "HOD", "Technician" }, user.Role);
                        return View(user);
                    }
                }

                // Check if username has changed and duplicates
                if (!string.Equals(existingUser.UserName, user.UserName, StringComparison.OrdinalIgnoreCase))
                {
                    var usernameExists = _context.Users.Any(u => u.UserName == user.UserName && u.ID != user.ID);
                    if (usernameExists)
                    {
                        ModelState.AddModelError("UserName", "Username is already in use by another user.");
                        ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name", user.DepartmentID);
                        ViewBag.Roles = new SelectList(new[] { "Student", "Lecturer", "HOD", "Technician" }, user.Role);
                        return View(user);
                    }
                }



                // Update user information
                existingUser.Role = user.Role; // Role selected from dropdown
                existingUser.DepartmentID = user.DepartmentID;
                _context.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while updating the user. Please try again.");
                ModelState.AddModelError("", $"Error Details: {ex.Message}");
                ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name", user.DepartmentID);
                ViewBag.Roles = new SelectList(new[] { "Student", "Lecturer", "HOD", "Technician" }, user.Role);
                return View(user);
            }
        }




        // Activate/Deactivate User
        public ActionResult ToggleStatus(int id)
        {
            var user = _context.Users.Find(id);
            if (user != null)
            {
                user.Status = !user.Status;
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
