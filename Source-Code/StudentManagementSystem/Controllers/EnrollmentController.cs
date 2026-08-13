using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Enrollment
        public async Task<IActionResult> Index()
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsNoTracking()
                .OrderByDescending(e => e.EnrollmentDate)
                .ToListAsync();

            return View(enrollments);
        }

        // GET: /Enrollment/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // GET: /Enrollment/Create
        public async Task<IActionResult> Create()
        {
            await LoadDropdowns();
            return View();
        }

        // POST: /Enrollment/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Enrollment enrollment)
        {
            bool alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.StudentId == enrollment.StudentId &&
                    e.CourseId == enrollment.CourseId &&
                    e.Status == "Active");

            if (alreadyEnrolled)
            {
                ModelState.AddModelError(
                    "",
                    "This student is already actively enrolled in this course."
                );
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(enrollment.StudentId, enrollment.CourseId);
                return View(enrollment);
            }

            _context.Enrollments.Add(enrollment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Student enrolled successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Enrollment/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments.FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            await LoadDropdowns(enrollment.StudentId, enrollment.CourseId);

            return View(enrollment);
        }

        // POST: /Enrollment/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Enrollment enrollment)
        {
            if (id != enrollment.EnrollmentId)
            {
                return NotFound();
            }

            bool alreadyEnrolled = await _context.Enrollments
                .AnyAsync(e =>
                    e.StudentId == enrollment.StudentId &&
                    e.CourseId == enrollment.CourseId &&
                    e.EnrollmentId != enrollment.EnrollmentId &&
                    e.Status == "Active");

            if (alreadyEnrolled)
            {
                ModelState.AddModelError(
                    "",
                    "This student is already actively enrolled in this course."
                );
            }

            if (!ModelState.IsValid)
            {
                await LoadDropdowns(enrollment.StudentId, enrollment.CourseId);
                return View(enrollment);
            }

            var existingEnrollment = await _context.Enrollments
                .FindAsync(id);

            if (existingEnrollment == null)
            {
                return NotFound();
            }

            existingEnrollment.StudentId = enrollment.StudentId;
            existingEnrollment.CourseId = enrollment.CourseId;
            existingEnrollment.EnrollmentDate = enrollment.EnrollmentDate;
            existingEnrollment.Status = enrollment.Status;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Enrollment updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Enrollment/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var enrollment = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EnrollmentId == id);

            if (enrollment == null)
            {
                return NotFound();
            }

            return View(enrollment);
        }

        // POST: /Enrollment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var enrollment = await _context.Enrollments
                .FindAsync(id);

            if (enrollment == null)
            {
                return NotFound();
            }

            _context.Enrollments.Remove(enrollment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Enrollment deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdowns(
            int? selectedStudentId = null,
            int? selectedCourseId = null)
        {
            var students = await _context.Students
                .AsNoTracking()
                .OrderBy(s => s.RegistrationNumber)
                .ToListAsync();

            var courses = await _context.Courses
                .AsNoTracking()
                .OrderBy(c => c.CourseCode)
                .ToListAsync();

            ViewBag.Students = new SelectList(
                students,
                "StudentId",
                "RegistrationNumber",
                selectedStudentId
            );

            ViewBag.Courses = new SelectList(
                courses,
                "CourseId",
                "CourseCode",
                selectedCourseId
            );
        }
    }
}