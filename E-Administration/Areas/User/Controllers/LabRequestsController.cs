﻿using E_Administration.Data;
using E_Administration.Dto;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace E_Administration.Areas.User.Controllers
{
    [Area("User")]
    public class LabRequestsController : Controller
    {
        private readonly DemoDbContext _context;

        public LabRequestsController(DemoDbContext context)
        {
            _context = context;
        }

        // GET: LabRequests/Create
        public IActionResult Create()
        {
            // Pass available departments to the view
            ViewBag.Departments = _context.Departments.ToList();
            return View();
        }

        // POST: LabRequests/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LabRequestDto labRequestDto)
        {
            // Lấy thông tin UserID từ Claims (xác nhận người dùng đã đăng nhập)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
            {
                ModelState.AddModelError(string.Empty, "User ID is missing or invalid. Please log in again.");
                PopulateDepartments();
                return View(labRequestDto);  // Trả lại DTO nếu có lỗi
            }

            // Kiểm tra nếu DepartmentID hoặc RequestedByID không hợp lệ
            if (labRequestDto.DepartmentID == 0)
            {
                ModelState.AddModelError("DepartmentID", "Please select a department.");
            }

            if (userId == 0)
            {
                ModelState.AddModelError("RequestedByID", "RequestedByID is required.");
            }

            if (ModelState.IsValid)
            {
                // Mapping dữ liệu từ DTO sang Entity
                var labRequest = new LabRequests
                {
                    DepartmentID = labRequestDto.DepartmentID,
                    RequestedByID = userId,  // Gán ID người yêu cầu từ Claims
                    Purpose = labRequestDto.Purpose,
                    AdminResponse = labRequestDto.AdminResponse,
                    Status = "Pending",  // Trạng thái mặc định
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                // Lưu vào database
                await _context.LabRequests.AddAsync(labRequest);
                await _context.SaveChangesAsync();

                return RedirectToAction("MyRequests");
            }

            // Reload departments if validation fails
            PopulateDepartments();
            return View(labRequestDto);  // Trả lại DTO nếu có lỗi
        }





        // Helper method to populate departments
        private void PopulateDepartments()
        {
            ViewBag.Departments = _context.Departments.ToList();
        }


        // Optional: List the logged-in user's lab requests
        public async Task<IActionResult> MyRequests()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "UserID")?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
                return RedirectToAction("Login", "Account");

            var requests = await _context.LabRequests
                .Include(lr => lr.Department)
                .Where(lr => lr.RequestedByID == userId)
                .ToListAsync();

            return View(requests);
        }

        // GET: Edit
        public IActionResult Edit(int id)
        {
            var labRequest = _context.LabRequests.Find(id);
            if (labRequest == null)
            {
                return NotFound();
            }

            // Map LabRequest to LabRequestDto
            var labRequestDto = new LabRequestDto
            {
                ID = labRequest.ID,
                DepartmentID = labRequest.DepartmentID,
                RequestedByID = labRequest.RequestedByID,
                Purpose = labRequest.Purpose,
                AdminResponse = labRequest.AdminResponse,
                Status = labRequest.Status,
                CreatedAt = labRequest.CreatedAt,
                UpdatedAt = labRequest.UpdatedAt
            };

            return View(labRequestDto);
        }

        // POST: Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, LabRequestDto model)
        {
            if (id != model.ID)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var labRequest = _context.LabRequests.Find(id);
                if (labRequest == null)
                {
                    return NotFound();
                }

                // Update only the Purpose field
                labRequest.Purpose = model.Purpose;
                labRequest.UpdatedAt = DateTime.Now; // Update the timestamp

                _context.Update(labRequest);
                _context.SaveChanges();

                return RedirectToAction("MyRequests", "LabRequests");
            }

            return View(model);
        }
        // POST: Delete
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var request = _context.LabRequests.Find(id);
            if (request != null)
            {
                _context.LabRequests.Remove(request);
                _context.SaveChanges();
            }
            return RedirectToAction("MyRequests");
        }

    }
}
