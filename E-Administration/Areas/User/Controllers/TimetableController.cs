using E_Administration.Data;
using E_Administration.Extensions;
using E_Administration.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Controllers
{
    [Area("User")]
    public class TimetableController : Controller
    {
       
        private readonly DemoDbContext _context;

        public TimetableController(DemoDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Timetable(int labId, DateTime? startDate, DateTime? endDate, int? year, string? weekNumber)
        {
            // Lấy thông tin phòng thí nghiệm
            var lab = await _context.Labs.FirstOrDefaultAsync(l => l.ID == labId);
            if (lab == null) return NotFound("Lab not found.");

            // Xử lý năm được chọn
            var selectedYear = year ?? DateTime.Now.Year;
            var weeksInYear = GetWeeksInYear(selectedYear);

            // Tính toán tuần
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

            // Lấy dữ liệu assignments
            var assignments = await _context.Assignments
                .Include(a => a.User)
                .Include(a => a.Lab)
                .Where(a => a.LabID == labId && a.Date >= startDate && a.Date <= endDate)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.TimeStart)
                .ToListAsync();

            // Xử lý thời khóa biểu
            var timetable = GenerateTimetable(assignments, startDate.Value);

            // Truyền dữ liệu vào ViewBag
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

        // Hàm xử lý thời khóa biểu
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

        // Hàm tính ngày trong tuần
        private List<string> GetWeekDays(DateTime startOfWeek)
        {
            return Enumerable.Range(0, 7)
                .Select(d => startOfWeek.AddDays(d).ToString("yyyy-MM-dd"))
                .ToList();
        }

        // Hàm tạo tên và ngày
        private List<object> GenerateDayNamesWithDates(DateTime startOfWeek)
        {
            var weekDays = new List<string> { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" };
            return weekDays.Select((day, index) => new
            {
                Day = day,
                Date = startOfWeek.AddDays(index).ToString("yyyy-MM-dd")
            }).Cast<object>().ToList(); // Dùng Cast<object> để chuyển đổi
        }




        // Hàm để tính tuần của năm
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
                    endOfWeekDate = lastDayOfYear; // Đảm bảo tuần cuối không vượt quá ngày cuối năm
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

