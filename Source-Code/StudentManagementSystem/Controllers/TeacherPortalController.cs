using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherPortalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherPortalController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpGet]
        public IActionResult Index() => RedirectToAction(nameof(Dashboard));

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return View("NoProfile");

            var courses = await _context.Courses
                .AsNoTracking()
                .Where(c => c.TeacherId == teacher.TeacherId)
                .OrderBy(c => c.CourseCode).ToListAsync();

            var courseIds = courses.Select(c => c.CourseId).ToList();

            var totalStudents = await _context.Enrollments.AsNoTracking()
                .Where(e => courseIds.Contains(e.CourseId))
                .Select(e => e.StudentId).Distinct().CountAsync();

            var totalGrades = await _context.Grades.AsNoTracking()
                .Where(g => courseIds.Contains(g.CourseId)).CountAsync();

            ViewBag.Teacher = teacher;
            ViewBag.Courses = courses;
            ViewBag.TotalStudents = totalStudents;
            ViewBag.TotalGrades = totalGrades;

            return View("Dashboard");
        }

        [HttpGet]
        public async Task<IActionResult> Courses()
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return View("NoProfile");

            var courses = await _context.Courses
                .AsNoTracking()
                .Where(c => c.TeacherId == teacher.TeacherId)
                .OrderBy(c => c.CourseCode).ToListAsync();

            var studentCounts = await _context.Enrollments.AsNoTracking()
                .Where(e => courses.Select(c => c.CourseId).Contains(e.CourseId))
                .GroupBy(e => e.CourseId)
                .Select(g => new { CourseId = g.Key, Count = g.Select(e => e.StudentId).Distinct().Count() })
                .ToDictionaryAsync(x => x.CourseId, x => x.Count);

            ViewBag.StudentCounts = studentCounts;
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> CourseRoster(int id)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return View("NoProfile");

            var course = await _context.Courses.AsNoTracking()
                .FirstOrDefaultAsync(c => c.CourseId == id && c.TeacherId == teacher.TeacherId);

            if (course == null) return Forbid();

            var enrollments = await _context.Enrollments.AsNoTracking()
                .Include(e => e.Student)
                .Where(e => e.CourseId == id)
                .OrderBy(e => e.Student!.FirstName).ToListAsync();

            var studentIds = enrollments.Select(e => e.StudentId).ToList();

            var grades = await _context.Grades.AsNoTracking()
                .Where(g => g.CourseId == id && studentIds.Contains(g.StudentId)).ToListAsync();

            ViewBag.Course = course;
            ViewBag.Grades = grades;

            return View(enrollments);
        }

        [HttpGet]
        public async Task<IActionResult> Grades()
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return View("NoProfile");

            var courseIds = await _context.Courses.AsNoTracking()
                .Where(c => c.TeacherId == teacher.TeacherId)
                .Select(c => c.CourseId).ToListAsync();

            var grades = await _context.Grades.AsNoTracking()
                .Include(g => g.Student).Include(g => g.Course)
                .Where(g => courseIds.Contains(g.CourseId))
                .OrderByDescending(g => g.GradeId).ToListAsync();

            return View(grades);
        }

        [HttpGet]
        public async Task<IActionResult> CreateGrade(int studentId, int courseId)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return View("NoProfile");

            var authorized = await IsTeacherCourseAsync(teacher.TeacherId, courseId)
                && await _context.Enrollments.AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);

            if (!authorized) return Forbid();

            var student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == studentId);
            var course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.CourseId == courseId);

            ViewBag.Student = student;
            ViewBag.Course = course;
            ViewBag.AssessmentTypes = GradingHelper.AssessmentTypes;

            var grade = new Grade { StudentId = studentId, CourseId = courseId };
            return View(grade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateGrade(Grade grade)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return View("NoProfile");

            var authorized = await IsTeacherCourseAsync(teacher.TeacherId, grade.CourseId)
                && await _context.Enrollments.AnyAsync(e => e.StudentId == grade.StudentId && e.CourseId == grade.CourseId);

            if (!authorized) return Forbid();

            if (grade.MarksObtained > grade.TotalMarks)
                ModelState.AddModelError(nameof(grade.MarksObtained), "Marks obtained cannot exceed total marks.");

            if (!ModelState.IsValid)
            {
                ViewBag.Student = await _context.Students.AsNoTracking().FirstOrDefaultAsync(s => s.StudentId == grade.StudentId);
                ViewBag.Course = await _context.Courses.AsNoTracking().FirstOrDefaultAsync(c => c.CourseId == grade.CourseId);
                ViewBag.AssessmentTypes = GradingHelper.AssessmentTypes;
                return View(grade);
            }

            grade.GradeLetter = GradingHelper.CalculateGradeLetter(grade.MarksObtained, grade.TotalMarks);

            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Grade recorded successfully.";
            return RedirectToAction(nameof(CourseRoster), new { id = grade.CourseId });
        }

        [HttpGet]
        public async Task<IActionResult> EditGrade(int id)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return View("NoProfile");

            var grade = await _context.Grades
                .Include(g => g.Student).Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.GradeId == id);

            if (grade == null) return NotFound();
            if (!await IsTeacherCourseAsync(teacher.TeacherId, grade.CourseId)) return Forbid();

            ViewBag.AssessmentTypes = GradingHelper.AssessmentTypes;
            return View(grade);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditGrade(int id, Grade grade)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return View("NoProfile");
            if (id != grade.GradeId) return NotFound();
            if (!await IsTeacherCourseAsync(teacher.TeacherId, grade.CourseId)) return Forbid();

            if (grade.MarksObtained > grade.TotalMarks)
                ModelState.AddModelError(nameof(grade.MarksObtained), "Marks obtained cannot exceed total marks.");

            if (!ModelState.IsValid)
            {
                ViewBag.AssessmentTypes = GradingHelper.AssessmentTypes;
                return View(grade);
            }

            var existingGrade = await _context.Grades.FindAsync(id);
            if (existingGrade == null) return NotFound();

            existingGrade.AssessmentType = grade.AssessmentType;
            existingGrade.MarksObtained = grade.MarksObtained;
            existingGrade.TotalMarks = grade.TotalMarks;
            existingGrade.Remarks = grade.Remarks;
            existingGrade.GradeLetter = GradingHelper.CalculateGradeLetter(grade.MarksObtained, grade.TotalMarks);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Grade updated successfully.";
            return RedirectToAction(nameof(CourseRoster), new { id = existingGrade.CourseId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteGrade(int id)
        {
            var teacher = await GetCurrentTeacherAsync();
            if (teacher == null) return View("NoProfile");

            var grade = await _context.Grades.FindAsync(id);
            if (grade == null) return NotFound();
            if (!await IsTeacherCourseAsync(teacher.TeacherId, grade.CourseId)) return Forbid();

            var courseId = grade.CourseId;

            _context.Grades.Remove(grade);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Grade removed successfully.";
            return RedirectToAction(nameof(CourseRoster), new { id = courseId });
        }

        private async Task<bool> IsTeacherCourseAsync(int teacherId, int courseId)
        {
            return await _context.Courses.AnyAsync(c => c.CourseId == courseId && c.TeacherId == teacherId);
        }

        private async Task<Teacher?> GetCurrentTeacherAsync()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrWhiteSpace(userId)) return null;

            return await _context.Teachers.AsNoTracking().FirstOrDefaultAsync(t => t.UserId == userId);
        }
    }
}