using E_Administration.Data;
using E_Administration.Dto;
using E_Administration.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace E_Administration.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class LabController : Controller
    {
        private readonly DemoDbContext ctx;

        public LabController(DemoDbContext ctx)
        {
            this.ctx = ctx;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var labs = await ctx.Labs
        .Include(l => l.Department) // Eager Loading thông tin từ bảng Department
        .ToListAsync();
            return View(labs);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var lab = await ctx.Labs
                .Include(x => x.Equipments) // Eager Loading: Lấy luôn dữ liệu từ bảng Equipment liên quan
                .SingleOrDefaultAsync(x => x.ID == id);
            if (lab == null)
            {
                return NotFound();
            }
            return View(lab);
        }

        [HttpGet]
        public async Task<IActionResult> ListLabs()
        {
            var labs = await ctx.Labs.ToListAsync();
            var departments = await ctx.Departments.ToListAsync();// Lấy danh sách từ database
            var viewModel = new LabViewModel
            {
                Labs = labs,
                Lab = new Lab(),
                Departments = departments// Đối tượng rỗng cho form thêm mới
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] Lab lab)
        {

            // Tìm Department dựa trên DepartmentID được gửi từ form
            var department = await ctx.Departments.FindAsync(lab.DepartmentID);
            if (department == null)
            {
                return Json(new { success = false, error = "Department không tồn tại." });
            }

            // Gắn Department vào Lab
            lab.Department = department;

            lab.CreatedAt = DateTime.Now;
            lab.UpdatedAt = DateTime.Now;

            // Lưu Lab mới vào cơ sở dữ liệu
            await ctx.Labs.AddAsync(lab);
            await ctx.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> DetailsAjax(int id)
        {
            var lab = await ctx.Labs.SingleOrDefaultAsync(x => x.ID == id);

            if (lab == null)
            {
                return Json(new { success = false, error = "Lab not found." });
            }

            return Json(new
            {
                success = true,
                id = lab.ID,
                name = lab.Name,
                departmentID = lab.DepartmentID,
                description = lab.Description,
                location = lab.Location,
                capacity = lab.Capacity,
                isOperational = lab.IsOperational
            });
        }


        [HttpPost]
        public async Task<IActionResult> EditAjax([FromBody] Lab lab)
        {
            var existingLab = await ctx.Labs.FindAsync(lab.ID);
            if (existingLab == null)
            {
                return Json(new { success = false, error = "Lab not found." });
            }

            // Update fields
            existingLab.Name = lab.Name;
            existingLab.DepartmentID = lab.DepartmentID;
            existingLab.Description = lab.Description;
            existingLab.Location = lab.Location;
            existingLab.Capacity = lab.Capacity;
            existingLab.IsOperational = lab.IsOperational;

            await ctx.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> ChangeStatusAjax([FromBody] ChangeStatusDto dto)
        {
            var lab = await ctx.Labs.FindAsync(dto.Id);
            if (lab == null)
            {
                return Json(new { success = false, error = "Lab not found." });
            }

            lab.IsOperational = dto.IsOperational;
            await ctx.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpGet]
        public async Task<IActionResult> SearchLabs(string query)
        {
            var results = string.IsNullOrWhiteSpace(query)
         ? await ctx.Labs
             .Include(lab => lab.Department) // Include để lấy thông tin Department
             .Select(lab => new
             {
                 lab.ID,
                 lab.Name,
                 DepartmentName = lab.Department.Name, // Lấy tên Department
                 lab.Location,
                 lab.Capacity,
                 lab.IsOperational
             })
             .ToListAsync()
         : await ctx.Labs
             .Include(lab => lab.Department) // Include để lấy thông tin Department
             .Where(lab => lab.Name.Contains(query) || lab.Location.Contains(query))
             .Select(lab => new
             {
                 lab.ID,
                 lab.Name,
                 DepartmentName = lab.Department.Name, // Lấy tên Department
                 lab.Location,
                 lab.Capacity,
                 lab.IsOperational
             })
             .ToListAsync();

            return Json(results);
        }



    }
}
