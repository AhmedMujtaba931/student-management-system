using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GradeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GradeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var grades = await _context.Grades
                .AsNoTracking()
                .Include(g => g.Student)
                .Include(g => g.Course)
                .OrderByDescending(g => g.GradeId)
                .ToListAsync();

            return View(grades);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var grade = await _context.Grades
                .AsNoTracking()
                .Include(g => g.Student)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.GradeId == id);

            if (grade == null) return NotFound();

            return View(grade);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropdownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Grade grade)
        {
            if (grade.MarksObtained > grade.TotalMarks)
                ModelState.AddModelError(nameof(grade.MarksObtained), "Marks obtained cannot exceed total marks.");

            bool studentExists = await _context.Students.AnyAsync(s => s.StudentId == grade.StudentId);
            bool courseExists = await _context.Courses.AnyAsync(c => c.CourseId == grade.CourseId);

            if (!studentExists) ModelState.AddModelError(nameof(grade.StudentId), "Select a valid student.");
            if (!courseExists) ModelState.AddModelError(nameof(grade.CourseId), "Select a valid course.");

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(grade.StudentId, grade.CourseId);
                return View(grade);
            }

            grade.GradeLetter = GradingHelper.CalculateGradeLetter(grade.MarksObtained, grade.TotalMarks);

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Grade recorded successfully.";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return NotFound();

            await LoadDropdownsAsync(grade.StudentId, grade.CourseId);
            return View(grade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Grade grade)
        {
            if (id != grade.GradeId) return NotFound();

            if (grade.MarksObtained > grade.TotalMarks)
                ModelState.AddModelError(nameof(grade.MarksObtained), "Marks obtained cannot exceed total marks.");

            if (!ModelState.IsValid)
            {
                await LoadDropdownsAsync(grade.StudentId, grade.CourseId);
                return View(grade);
            }

            try
            {
                var existingGrade = await _context.Grades.FindAsync(id);
                if (existingGrade == null) return NotFound();

                existingGrade.StudentId = grade.StudentId;
                existingGrade.CourseId = grade.CourseId;
                existingGrade.AssessmentType = grade.AssessmentType;
                existingGrade.MarksObtained = grade.MarksObtained;
                existingGrade.TotalMarks = grade.TotalMarks;
                existingGrade.Remarks = grade.Remarks;
                existingGrade.GradeLetter = GradingHelper.CalculateGradeLetter(grade.MarksObtained, grade.TotalMarks);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Grade updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Grades.AnyAsync(g => g.GradeId == id)) return NotFound();
                throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var grade = await _context.Grades
                .AsNoTracking()
                .Include(g => g.Student)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.GradeId == id);

            if (grade == null) return NotFound();

            return View(grade);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return NotFound();

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Grade deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadDropdownsAsync(object? selectedStudentId = null, object? selectedCourseId = null)
        {
            var students = await _context.Students
                .AsNoTracking()
                .OrderBy(s => s.FirstName)
                .Select(s => new { s.StudentId, FullName = s.FirstName + " " + s.LastName + " (" + s.RegistrationNumber + ")" })
                .ToListAsync();

            var courses = await _context.Courses
                .AsNoTracking()
                .OrderBy(c => c.CourseCode)
                .Select(c => new { c.CourseId, Display = c.CourseCode + " - " + c.CourseName })
                .ToListAsync();

            ViewBag.Students = new SelectList(students, "StudentId", "FullName", selectedStudentId);
            ViewBag.Courses = new SelectList(courses, "CourseId", "Display", selectedCourseId);
            ViewBag.AssessmentTypes = GradingHelper.AssessmentTypes;
        }
    }
}