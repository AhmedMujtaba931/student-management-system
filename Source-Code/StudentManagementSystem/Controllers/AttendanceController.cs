using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class AttendanceController : Controller
    {
        private readonly ApplicationDbContext _context;

        private static readonly string[] ValidStatuses =
        {
            "Present",
            "Absent",
            "Late",
            "Excused"
        };

        public AttendanceController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Attendance
        public async Task<IActionResult> Index()
        {
            var sessions = await _context.AttendanceSessions
                .Include(a => a.Course)
                .Include(a => a.AttendanceRecords)
                .AsNoTracking()
                .OrderByDescending(a => a.LectureDate)
                .ThenByDescending(a => a.LectureNumber)
                .ToListAsync();

            return View(sessions);
        }

        // GET: /Attendance/Create
        public async Task<IActionResult> Create()
        {
            await LoadCourses();
            return View(new AttendanceSession
            {
                LectureDate = DateTime.Today
            });
        }

        // POST: /Attendance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttendanceSession session)
        {
            if (session.EndTime <= session.StartTime)
            {
                ModelState.AddModelError(
                    nameof(session.EndTime),
                    "End time must be after start time.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCourses(session.CourseId);
                return View(session);
            }

            var lastLectureNumber = await _context.AttendanceSessions
                .Where(a => a.CourseId == session.CourseId)
                .Select(a => (int?)a.LectureNumber)
                .MaxAsync() ?? 0;

            session.LectureNumber = lastLectureNumber + 1;
            session.CreatedAt = DateTime.Now;

            _context.AttendanceSessions.Add(session);
            await _context.SaveChangesAsync();

            return RedirectToAction(
                nameof(Mark),
                new { id = session.AttendanceSessionId });
        }

        // GET: /Attendance/Mark/5
        public async Task<IActionResult> Mark(int id)
        {
            var session = await _context.AttendanceSessions
                .Include(a => a.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.AttendanceSessionId == id);

            if (session == null)
            {
                return NotFound();
            }

            var students = await _context.Enrollments
                .Where(e =>
                    e.CourseId == session.CourseId &&
                    e.Status == "Active")
                .Include(e => e.Student)
                .AsNoTracking()
                .OrderBy(e => e.Student!.RegistrationNumber)
                .Select(e => e.Student!)
                .ToListAsync();

            ViewBag.Students = students;

            var existingRecords = await _context.AttendanceRecords
                .Where(a => a.AttendanceSessionId == id)
                .ToDictionaryAsync(a => a.StudentId);

            ViewBag.ExistingRecords = existingRecords;

            return View(session);
        }

        // POST: /Attendance/Mark
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Mark(
            int id,
            Dictionary<int, string> statuses,
            Dictionary<int, string?> remarks)
        {
            var session = await _context.AttendanceSessions
                .FirstOrDefaultAsync(a => a.AttendanceSessionId == id);

            if (session == null)
            {
                return NotFound();
            }

            var studentIds = await _context.Enrollments
                .Where(e =>
                    e.CourseId == session.CourseId &&
                    e.Status == "Active")
                .Select(e => e.StudentId)
                .ToListAsync();

            var existingRecords = await _context.AttendanceRecords
                .Where(a => a.AttendanceSessionId == id)
                .ToListAsync();

            foreach (var studentId in studentIds)
            {
                var status = statuses.ContainsKey(studentId)
                    ? statuses[studentId]
                    : "Absent";

                if (!ValidStatuses.Contains(
                        status,
                        StringComparer.OrdinalIgnoreCase))
                {
                    status = "Absent";
                }

                var remark = remarks.ContainsKey(studentId)
                    ? remarks[studentId]
                    : null;

                var existing = existingRecords
                    .FirstOrDefault(a => a.StudentId == studentId);

                if (existing == null)
                {
                    _context.AttendanceRecords.Add(
                        new AttendanceRecord
                        {
                            AttendanceSessionId = id,
                            StudentId = studentId,
                            Status = status,
                            Remarks = string.IsNullOrWhiteSpace(remark)
                                ? null
                                : remark.Trim()
                        });
                }
                else
                {
                    existing.Status = status;
                    existing.Remarks =
                        string.IsNullOrWhiteSpace(remark)
                            ? null
                            : remark.Trim();
                }
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Attendance saved successfully.";

            return RedirectToAction(
                nameof(Details),
                new { id });
        }

        // GET: /Attendance/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var session = await _context.AttendanceSessions
                .Include(a => a.Course)
                .Include(a => a.AttendanceRecords)
                    .ThenInclude(a => a.Student)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    a => a.AttendanceSessionId == id);

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // GET: /Attendance/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var session = await _context.AttendanceSessions
                .FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            await LoadCourses(session.CourseId);

            return View(session);
        }

        // POST: /Attendance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            AttendanceSession session)
        {
            if (id != session.AttendanceSessionId)
            {
                return NotFound();
            }

            if (session.EndTime <= session.StartTime)
            {
                ModelState.AddModelError(
                    nameof(session.EndTime),
                    "End time must be after start time.");
            }

            if (!ModelState.IsValid)
            {
                await LoadCourses(session.CourseId);
                return View(session);
            }

            var existing = await _context.AttendanceSessions
                .FindAsync(id);

            if (existing == null)
            {
                return NotFound();
            }

            existing.CourseId = session.CourseId;
            existing.LectureDate = session.LectureDate;
            existing.StartTime = session.StartTime;
            existing.EndTime = session.EndTime;
            existing.Topic = session.Topic;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Lecture updated successfully.";

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: /Attendance/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var session = await _context.AttendanceSessions
                .Include(a => a.Course)
                .Include(a => a.AttendanceRecords)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    a => a.AttendanceSessionId == id);

            if (session == null)
            {
                return NotFound();
            }

            return View(session);
        }

        // POST: /Attendance/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var session = await _context.AttendanceSessions
                .FindAsync(id);

            if (session == null)
            {
                return NotFound();
            }

            _context.AttendanceSessions.Remove(session);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Lecture attendance deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCourses(int? selectedCourseId = null)
        {
            var courses = await _context.Courses
                .AsNoTracking()
                .OrderBy(c => c.CourseCode)
                .ToListAsync();

            ViewBag.Courses = new SelectList(
                courses,
                "CourseId",
                "CourseCode",
                selectedCourseId);
        }
    }
}