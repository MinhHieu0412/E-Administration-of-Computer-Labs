﻿
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
            // Lấy danh sách role để hiển thị trong dropdown
            ViewBag.Roles = _context.Users
                .Select(u => u.Role)
                .Distinct()
                .ToList();

            // Lưu giá trị roleFilter vào ViewBag để giữ lại lựa chọn trong dropdown
            ViewBag.RoleFilter = roleFilter;

            // Eager load Department bằng Include
            var users = _context.Users.Include(u => u.Department).AsQueryable();

            // Nếu có roleFilter, lọc theo role
            if (!string.IsNullOrEmpty(roleFilter))
            {
                users = users.Where(u => u.Role == roleFilter);
            }

            // Nếu có searchString, lọc theo username hoặc email
            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.UserName.Contains(searchString) || u.Email.Contains(searchString));
            }

            // Lưu giá trị searchString vào ViewData để giữ lại trong form tìm kiếm
            ViewData["SearchString"] = searchString;

            // Trả về danh sách users sau khi lọc
            return View(users.ToList());
        }




        // Create User (GET)
        public ActionResult Create()
        {
            // Truyền danh sách Departments và Roles vào View
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

        private string GenerateRandomPassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12).Select(s => s[random.Next(s.Length)]).ToArray());
        }

        private void SendAccountEmail(string email, string userName, string password)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("E_Administration", "caonguyen2288@gmail.com")); // Sử dụng email domain của bạn
            message.To.Add(new MailboxAddress(userName, email));
            message.Subject = "Welcome to E_Administration - Your Account Details";

            // Nội dung email HTML và Plain Text để cải thiện chất lượng
            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = $@"
            <h3>Hello {userName},</h3>
            <p>Your account has been successfully created.</p>
            <p><b>Username:</b> {userName}</p>
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
                    client.Authenticate("caonguyen2288@gmail.com", "jcxf ntvr buwh ibne"); // Sử dụng email và mật khẩu thật hoặc App Password

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
            return View(user);
        }

        // Edit User (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
                var existingUser = _context.Users.Find(user.ID);
                if (existingUser != null)
                {
                    existingUser.UserName = user.UserName;
                    existingUser.Email = user.Email;
                    // Hash the password only if it has changed
                    if (!string.IsNullOrEmpty(user.Password) &&
                        !BCrypt.Net.BCrypt.Verify(user.Password, existingUser.Password))
                    {
                        existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                    }
                    existingUser.Image = user.Image;
                    existingUser.Role = user.Role;
                    existingUser.DepartmentID = user.DepartmentID;
                    _context.SaveChanges();
                }
                return RedirectToAction("Index");
            }
            ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name", user.DepartmentID);
            return View(user);
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
