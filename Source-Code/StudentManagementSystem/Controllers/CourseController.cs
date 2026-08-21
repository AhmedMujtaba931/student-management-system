using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CourseController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CourseController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses
                .AsNoTracking().Include(c => c.Teacher)
                .OrderBy(c => c.CourseCode).ToListAsync();
            return View(courses);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses
                .AsNoTracking().Include(c => c.Teacher)
                .FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null) return NotFound();
            return View(course);
        }

        public async Task<IActionResult> Create()
        {
            await LoadTeachersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Course course)
        {
            bool courseCodeExists = await _context.Courses
                .AnyAsync(c => c.CourseCode == course.CourseCode);

            if (courseCodeExists)
                ModelState.AddModelError(nameof(course.CourseCode), "This course code already exists.");

            if (!ModelState.IsValid)
            {
                await LoadTeachersAsync(course.TeacherId);
                return View(course);
            }

            course.CreatedAt = DateTime.Now;
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Course added successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            await LoadTeachersAsync(course.TeacherId);
            return View(course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Course course)
        {
            if (id != course.CourseId) return NotFound();

            bool courseCodeExists = await _context.Courses
                .AnyAsync(c => c.CourseCode == course.CourseCode && c.CourseId != course.CourseId);

            if (courseCodeExists)
                ModelState.AddModelError(nameof(course.CourseCode), "This course code already belongs to another course.");

            if (!ModelState.IsValid)
            {
                await LoadTeachersAsync(course.TeacherId);
                return View(course);
            }

            try
            {
                var existingCourse = await _context.Courses.FindAsync(id);
                if (existingCourse == null) return NotFound();

                existingCourse.CourseCode = course.CourseCode;
                existingCourse.CourseName = course.CourseName;
                existingCourse.CreditHours = course.CreditHours;
                existingCourse.Description = course.Description;
                existingCourse.TeacherId = course.TeacherId;

                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CourseExists(course.CourseId)) return NotFound();
                throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var course = await _context.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();

            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Course deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> CourseExists(int id)
        {
            return await _context.Courses.AnyAsync(c => c.CourseId == id);
        }

        private async Task LoadTeachersAsync(object? selectedTeacherId = null)
        {
            var teachers = await _context.Teachers.AsNoTracking()
                .OrderBy(t => t.FirstName)
                .Select(t => new { t.TeacherId, FullName = t.FirstName + " " + t.LastName + " (" + t.EmployeeCode + ")" })
                .ToListAsync();

            ViewBag.Teachers = new SelectList(teachers, "TeacherId", "FullName", selectedTeacherId);
        }
    }
}