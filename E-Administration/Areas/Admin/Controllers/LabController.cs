using E_Administration.Data;
using E_Administration.Dto;
using E_Administration.Extensions;
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
                .Include(x => x.Equipments)
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
            var departments = await ctx.Departments.ToListAsync();
            var viewModel = new LabViewModel
            {
                Labs = labs,
                Lab = new Lab(),
                Departments = departments
            };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<IActionResult> CreateAjax([FromBody] Lab lab)
        {

            // Find Department based on DepartmentID sent from form
            var department = await ctx.Departments.FindAsync(lab.DepartmentID);
            if (department == null)
            {
                return Json(new { success = false, error = "Department không tồn tại." });
            }

            
            lab.Department = department;

            lab.CreatedAt = DateTime.Now;
            lab.UpdatedAt = DateTime.Now;

            
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


        public async Task<IActionResult> Timetable(int labId, DateTime? startDate, DateTime? endDate, int? year, string? weekNumber)
        {
            // Get lab information
            var lab = await ctx.Labs.FirstOrDefaultAsync(l => l.ID == labId);
            if (lab == null) return NotFound("Lab not found.");

            // Process selected year
            var selectedYear = year ?? DateTime.Now.Year;
            var weeksInYear = GetWeeksInYear(selectedYear);

            // Weekly calculation
            if (!string.IsNullOrEmpty(weekNumber))
            {
                var dates = weekNumber.Split(" - ");
                if (dates.Length == 2 && DateTime.TryParse(dates[0], out var parsedStartDate) && DateTime.TryParse(dates[1], out var parsedEndDate))
                {
                    startDate = parsedStartDate;
                    endDate = parsedEndDate;
                }
                else
                {
                    return BadRequest("Invalid week format.");
                }
            }

            if (!startDate.HasValue || !endDate.HasValue)
            {
                startDate = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
                endDate = startDate.Value.AddDays(6);
            }

            // Get assignments data
            var assignments = await ctx.Assignments
                .Include(a => a.User)
                .Include(a => a.Lab)
                .Where(a => a.LabID == labId && a.Date >= startDate && a.Date <= endDate)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.TimeStart)
                .ToListAsync();

            
            var timetable = GenerateTimetable(assignments, startDate.Value);

            // Passing data into ViewBag
            ViewBag.WeeksInYear = weeksInYear;
            ViewBag.WeekDays = GetWeekDays(startDate.Value);
            ViewBag.LabId = labId;
            ViewBag.LabName = lab.Name;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.StartDate = startDate.Value.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate.Value.ToString("yyyy-MM-dd");
            ViewBag.DayNamesWithDates = GenerateDayNamesWithDates(startDate.Value);

            return View(timetable);
        }

        // Timetable processing function
        private List<List<dynamic>> GenerateTimetable(List<Assignments> assignments, DateTime startOfWeek)
        {
            var timetable = new List<List<dynamic>>();
            for (var day = 0; day < 7; day++)
            {
                var currentDay = startOfWeek.AddDays(day);
                var dayAssignments = assignments.Where(a => a.Date.Date == currentDay.Date).ToList();

                var daySchedule = new List<dynamic>();
                for (var hour = 8; hour <= 17; hour++)
                {
                    var assignmentInSlot = dayAssignments.FirstOrDefault(a => hour >= a.TimeStart.Hours && hour < a.TimeEnd.Hours);
                    daySchedule.Add(new
                    {
                        Time = $"{hour}:00",
                        Assignment = assignmentInSlot != null ? $"{assignmentInSlot.User?.UserName} - {assignmentInSlot.Notes}" : "",
                        Status = assignmentInSlot != null && DateTime.Now > assignmentInSlot.Date.Add(assignmentInSlot.TimeEnd) ? "Completed" : "Not Completed"
                    });
                }
                timetable.Add(daySchedule);
            }
            return timetable;
        }

        // Day of the week function
        private List<string> GetWeekDays(DateTime startOfWeek)
        {
            return Enumerable.Range(0, 7)
                .Select(d => startOfWeek.AddDays(d).ToString("yyyy-MM-dd"))
                .ToList();
        }

        // Name and date constructor
        private List<object> GenerateDayNamesWithDates(DateTime startOfWeek)
        {
            var weekDays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            return weekDays.Select((day, index) => new
            {
                Day = day,
                Date = startOfWeek.AddDays(index).ToString("yyyy-MM-dd")
            }).Cast<object>().ToList(); // Dùng Cast<object> để chuyển đổi
        }




        // Function to calculate week of year
        private List<(string startDate, string endDate)> GetWeeksInYear(int year)
        {
            var weeks = new List<(string startDate, string endDate)>();
            var firstDayOfYear = new DateTime(year, 1, 1);
            var lastDayOfYear = new DateTime(year, 12, 31);
            var startOfWeek = firstDayOfYear.StartOfWeek(DayOfWeek.Monday);

            while (startOfWeek <= lastDayOfYear)
            {
                var endOfWeekDate = startOfWeek.AddDays(6);
                if (endOfWeekDate > lastDayOfYear)
                {
                    endOfWeekDate = lastDayOfYear; 
                }

                weeks.Add((startOfWeek.ToString("yyyy-MM-dd"), endOfWeekDate.ToString("yyyy-MM-dd")));
                startOfWeek = startOfWeek.AddDays(7);
            }

            return weeks;
        }

        [HttpGet]
        public IActionResult GetWeeksByYear(int year)
        {
            var weeks = GetWeeksInYear(year);
            return Json(weeks.Select(week => new
            {
                startDate = week.startDate,
                endDate = week.endDate
            }));
        }
    }
}
