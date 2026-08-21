using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public AdminController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.StudentCount = await _context.Students.CountAsync();
            ViewBag.CourseCount = await _context.Courses.CountAsync();
            ViewBag.EnrollmentCount = await _context.Enrollments.CountAsync();
            ViewBag.GradeCount = await _context.Grades.CountAsync();
            ViewBag.TeacherCount = (await _userManager.GetUsersInRoleAsync("Teacher")).Count;
            return View();
        }

        public async Task<IActionResult> Reports()
        {
            ViewBag.ByCourse = await _context.Enrollments.AsNoTracking()
                .Include(e => e.Course)
                .GroupBy(e => e.Course!.CourseCode + " - " + e.Course.CourseName)
                .Select(g => new { Course = g.Key, Count = g.Count() })
                .OrderByDescending(x => x.Count).ToListAsync();

            ViewBag.GradeDist = await _context.Grades.AsNoTracking()
                .GroupBy(g => g.GradeLetter ?? "N/A")
                .Select(g => new { Letter = g.Key, Count = g.Count() })
                .OrderBy(x => x.Letter).ToListAsync();

            return View();
        }

        public async Task<IActionResult> Teachers()
        {
            var teachers = await _userManager.GetUsersInRoleAsync("Teacher");
            return View(teachers.OrderBy(t => t.FullName).ToList());
        }

        [HttpGet]
        public IActionResult CreateTeacher() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTeacher(string fullName, string email, string password)
        {
            if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                TempData["Error"] = "All fields are required.";
                return View();
            }

            if (await _userManager.FindByEmailAsync(email) != null)
            {
                TempData["Error"] = "Email already registered.";
                return View();
            }

            var user = new ApplicationUser
            {
                UserName = email.Trim(),
                Email = email.Trim(),
                EmailConfirmed = true,
                FullName = fullName.Trim()
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                TempData["Error"] = string.Join(" ", result.Errors.Select(e => e.Description));
                return View();
            }

            await _userManager.AddToRoleAsync(user, "Teacher");
            TempData["Success"] = $"Teacher '{fullName}' created successfully.";
            return RedirectToAction(nameof(Teachers));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveTeacher(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null && await _userManager.IsInRoleAsync(user, "Teacher"))
            {
                await _userManager.RemoveFromRoleAsync(user, "Teacher");
                TempData["Success"] = "Teacher role removed.";
            }
            return RedirectToAction(nameof(Teachers));
        }

        public async Task<IActionResult> LinkStudent()
        {
            var unlinked = await _context.Students.AsNoTracking()
                .Where(s => s.UserId == null)
                .OrderBy(s => s.RegistrationNumber)
                .Select(s => new { s.StudentId, Name = s.RegistrationNumber + " - " + s.FirstName + " " + s.LastName })
                .ToListAsync();

            var studentUsers = await _userManager.GetUsersInRoleAsync("Student");
            var linkedIds = await _context.Students.Where(s => s.UserId != null).Select(s => s.UserId!).ToListAsync();
            var available = studentUsers.Where(u => !linkedIds.Contains(u.Id))
                .Select(u => new { u.Id, Name = u.FullName + " (" + u.Email + ")" }).ToList();

            ViewBag.Students = new SelectList(unlinked, "StudentId", "Name");
            ViewBag.Users = new SelectList(available, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkStudent(int studentId, string userId)
        {
            var student = await _context.Students.FindAsync(studentId);
            if (student == null) return NotFound();
            student.UserId = userId;
            await _context.SaveChangesAsync();
            TempData["Success"] = "Student linked successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}