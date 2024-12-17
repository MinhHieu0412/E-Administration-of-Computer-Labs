
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc;
using E_Administration.Data;
using Microsoft.EntityFrameworkCore;

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
        public ActionResult Index()
        {
            // Eager load Department bằng Include
            var users = _context.Users.Include(u => u.Department).ToList();
            return View(users);
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
            // Kiểm tra nếu Email đã tồn tại
            if (_context.Users.Any(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email already exists. Please use another email.");
                ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name");
                ViewBag.Roles = new SelectList(new List<string> { "Student", "Lecturer", "HOD", "Technician" });
                return View(user);
            }

            // Kiểm tra nếu UserName đã tồn tại
            if (_context.Users.Any(u => u.UserName == user.UserName))
            {
                ModelState.AddModelError("UserName", "Username already exists. Please choose another one.");
                ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name");
                ViewBag.Roles = new SelectList(new List<string> { "Student", "Lecturer", "HOD", "Technician" });
                return View(user);
            }

            try {   
                user.Status = true; // Đặt trạng thái mặc định là Active
                _context.Users.Add(user);
                _context.SaveChanges();
                return RedirectToAction("Index");
            }
            catch (Exception ex) { 
                // Truyền lại danh sách Departments và Roles khi có lỗi
            ViewBag.Departments = new SelectList(_context.Departments, "ID", "Name");
            ViewBag.Roles = new SelectList(new List<string> { "Student", "Lecturer", "HOD", "Technician" });
            return View(user);
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
                    existingUser.Password = user.Password;
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
