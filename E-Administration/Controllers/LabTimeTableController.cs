using E_Administration.Data;
using E_Administration.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Administration.Controllers
{
    public class LabTimeTableController : Controller
    {
        private readonly DemoDbContext _context;

        public LabTimeTableController(DemoDbContext context)
        {
            _context = context;
        }

        // Action để hiển thị timetable theo LabID
        public async Task<IActionResult> Timetable(int labId, int? weekNumber)
        {
            labId = 1;
            // Lấy thông tin Lab từ cơ sở dữ liệu
            var lab = await _context.Labs.FirstOrDefaultAsync(l => l.ID == labId);
            if (lab == null)
            {
                return NotFound("Lab not found.");
            }

            // Nếu không có weekNumber, mặc định là tuần hiện tại
            var currentWeekNumber = weekNumber ?? GetWeekOfYear(DateTime.Now);

            // Lấy ngày đầu tiên của năm hiện tại
            var firstDayOfYear = new DateTime(DateTime.Now.Year, 1, 1);
            // Tính ngày bắt đầu tuần (thứ Hai)
            var startOfWeek = firstDayOfYear.AddDays((currentWeekNumber - 1) * 7).StartOfWeek(DayOfWeek.Monday);
            var endOfWeek = startOfWeek.AddDays(6); // Ngày kết thúc tuần (Chủ Nhật)

            // Lấy tất cả các Assignment trong tuần cho Lab cụ thể
            var assignments = await _context.Assignments
                .Include(a => a.User)
                .Include(a => a.Lab)
                .Where(a => a.LabID == labId && a.Date >= startOfWeek && a.Date <= endOfWeek)
                .OrderBy(a => a.Date)
                .ThenBy(a => a.TimeStart)
                .ToListAsync();

            // Xử lý timetable
            var timetable = new List<List<dynamic>>();
            for (var day = 0; day < 7; day++) // Lặp qua các ngày từ Monday đến Sunday
            {
                var currentDay = startOfWeek.AddDays(day);
                var dayAssignments = assignments.Where(a => a.Date.Date == currentDay.Date).ToList();

                var daySchedule = new List<dynamic>();
                for (var hour = 8; hour <= 17; hour++) // Lặp qua từng giờ từ 8:00 đến 17:00
                {
                    var assignmentInSlot = dayAssignments.FirstOrDefault(a =>
                        hour >= a.TimeStart.Hours && hour < a.TimeEnd.Hours);

                    if (assignmentInSlot != null)
                    {
                        daySchedule.Add(new
                        {
                            Time = $"{hour}:00",
                            Assignment = $"{assignmentInSlot.User?.UserName} - {assignmentInSlot.Notes}",
                            Status = DateTime.Now > assignmentInSlot.Date.Add(assignmentInSlot.TimeEnd) ? "Completed" : "Not Completed"
                        });
                    }
                    else
                    {
                        daySchedule.Add(new
                        {
                            Time = $"{hour}:00",
                            Assignment = "",
                            Status = ""
                        });
                    }
                }
                timetable.Add(daySchedule);
            }

            // Truyền thông tin ngày của từng ngày trong tuần qua ViewBag
            ViewBag.WeekDays = Enumerable.Range(0, 7).Select(d => startOfWeek.AddDays(d).ToString("yyyy-MM-dd")).ToList();
            ViewBag.LabId = labId;
            ViewBag.LabName = lab.Name; // Truyền tên Lab vào ViewBag
            ViewBag.CurrentWeek = currentWeekNumber;
            ViewBag.StartOfWeek = startOfWeek.ToString("yyyy-MM-dd");
            ViewBag.EndOfWeek = endOfWeek.ToString("yyyy-MM-dd");
            return View(timetable);
        }


        // Hàm để tính tuần của năm
        private int GetWeekOfYear(DateTime date)
        {
            var culture = System.Globalization.CultureInfo.CurrentCulture;
            return culture.Calendar.GetWeekOfYear(date, System.Globalization.CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }


    }
}
