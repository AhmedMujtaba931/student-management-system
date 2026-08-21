using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentPortalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentPortalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return View("NoProfile");

            var enrollments = await _context.Enrollments.AsNoTracking()
                .Include(e => e.Course)
                .Where(e => e.StudentId == student.StudentId)
                .OrderByDescending(e => e.EnrollmentDate).ToListAsync();

            var attendance = await _context.AttendanceRecords.AsNoTracking()
                .Include(a => a.Enrollment).ThenInclude(e => e!.Course)
                .Where(a => a.Enrollment != null && a.Enrollment.StudentId == student.StudentId)
                .OrderByDescending(a => a.AttendanceDate).ToListAsync();

            var grades = await _context.Grades.AsNoTracking()
                .Include(g => g.Course)
                .Where(g => g.StudentId == student.StudentId)
                .OrderByDescending(g => g.GradeId).ToListAsync();

            ViewBag.Student = student;
            ViewBag.Enrollments = enrollments;
            ViewBag.Attendance = attendance;
            ViewBag.Grades = grades;

            return View("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return View("NoProfile");
            return View("Index", student);
        }

        [HttpGet]
        public async Task<IActionResult> Courses()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return View("NoProfile");

            var enrollments = await _context.Enrollments.AsNoTracking()
                .Include(e => e.Course)
                .Where(e => e.StudentId == student.StudentId)
                .OrderByDescending(e => e.EnrollmentDate).ToListAsync();

            return View(enrollments);
        }

        [HttpGet]
        public async Task<IActionResult> Attendance()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return View("NoProfile");

            var records = await _context.AttendanceRecords.AsNoTracking()
                .Include(a => a.Enrollment).ThenInclude(e => e!.Course)
                .Where(a => a.Enrollment != null && a.Enrollment.StudentId == student.StudentId)
                .OrderByDescending(a => a.AttendanceDate).ToListAsync();

            return View(records);
        }

        [HttpGet]
        public async Task<IActionResult> Grades()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null) return View("NoProfile");

            var grades = await _context.Grades.AsNoTracking()
                .Include(g => g.Course)
                .Where(g => g.StudentId == student.StudentId)
                .OrderByDescending(g => g.GradeId).ToListAsync();

            return View(grades);
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        private async Task<Student?> GetCurrentStudentAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId)) return null;

            return await _context.Students.AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }
    }
}