using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<HomeController> _logger;

        public HomeController(
            ApplicationDbContext context,
            ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var totalStudents = await _context.Students.CountAsync();
            var totalCourses = await _context.Courses.CountAsync();
            var totalEnrollments = await _context.Enrollments.CountAsync();
            var totalAttendance = await _context.AttendanceRecords.CountAsync();

            var presentAttendance = await _context.AttendanceRecords
                .CountAsync(a => a.Status == "Present");

            var attendancePercentage = totalAttendance > 0
                ? Math.Round(
                    (double)presentAttendance / totalAttendance * 100, 1)
                : 0;

            var totalGrades = await _context.Grades.CountAsync();

            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalCourses = totalCourses;
            ViewBag.TotalEnrollments = totalEnrollments;
            ViewBag.AttendancePercentage = attendancePercentage;
            ViewBag.TotalGrades = totalGrades;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(
            Duration = 0,
            Location = ResponseCacheLocation.None,
            NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id
                    ?? HttpContext.TraceIdentifier
            });
        }
    }
}
