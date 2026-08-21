using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
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
        .AsNoTracking()
        .Include(s => s.Course)
        .OrderByDescending(s => s.LectureDate)
        .ThenByDescending(s => s.LectureNumber)
        .ToListAsync();

    return View(sessions);
}

        // GET: /Attendance/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.AttendanceRecords
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e!.Student)
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e!.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    a => a.AttendanceRecordId == id);

            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        // GET: /Attendance/Create
        public async Task<IActionResult> Create()
        {
            await LoadEnrollments();
            return View();
        }

        // POST: /Attendance/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            AttendanceRecord record)
        {
            ValidateStatus(record.Status);

            var duplicate = await _context.AttendanceRecords
                .AnyAsync(a =>
                    a.EnrollmentId == record.EnrollmentId &&
                    a.AttendanceDate.Date ==
                    record.AttendanceDate.Date);

            if (duplicate)
            {
                ModelState.AddModelError(
                    "",
                    "Attendance for this student on this date already exists.");
            }

            if (!ModelState.IsValid)
            {
                await LoadEnrollments(record.EnrollmentId);
                return View(record);
            }

            record.AttendanceDate =
                record.AttendanceDate.Date;

            _context.AttendanceRecords.Add(record);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Attendance record added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Attendance/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.AttendanceRecords
                .FindAsync(id);

            if (record == null)
            {
                return NotFound();
            }

            await LoadEnrollments(record.EnrollmentId);

            return View(record);
        }

        // POST: /Attendance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            AttendanceRecord record)
        {
            if (id != record.AttendanceRecordId)
            {
                return NotFound();
            }

            ValidateStatus(record.Status);

            var duplicate = await _context.AttendanceRecords
                .AnyAsync(a =>
                    a.EnrollmentId == record.EnrollmentId &&
                    a.AttendanceDate.Date ==
                    record.AttendanceDate.Date &&
                    a.AttendanceRecordId !=
                    record.AttendanceRecordId);

            if (duplicate)
            {
                ModelState.AddModelError(
                    "",
                    "Attendance for this student on this date already exists.");
            }

            if (!ModelState.IsValid)
            {
                await LoadEnrollments(record.EnrollmentId);
                return View(record);
            }

            var existingRecord =
                await _context.AttendanceRecords.FindAsync(id);

            if (existingRecord == null)
            {
                return NotFound();
            }

            existingRecord.EnrollmentId =
                record.EnrollmentId;

            existingRecord.AttendanceDate =
                record.AttendanceDate.Date;

            existingRecord.Status =
                record.Status;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Attendance record updated successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Attendance/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var record = await _context.AttendanceRecords
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e!.Student)
                .Include(a => a.Enrollment)
                    .ThenInclude(e => e!.Course)
                .AsNoTracking()
                .FirstOrDefaultAsync(
                    a => a.AttendanceRecordId == id);

            if (record == null)
            {
                return NotFound();
            }

            return View(record);
        }

        // POST: /Attendance/Delete/5
        [HttpPost]
        [ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var record =
                await _context.AttendanceRecords.FindAsync(id);

            if (record == null)
            {
                return NotFound();
            }

            _context.AttendanceRecords.Remove(record);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Attendance record deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private void ValidateStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status) ||
                !ValidStatuses.Contains(
                    status,
                    StringComparer.OrdinalIgnoreCase))
            {
                ModelState.AddModelError(
                    nameof(AttendanceRecord.Status),
                    "Please select a valid attendance status.");
            }
        }

        private async Task LoadEnrollments(
            int? selectedEnrollmentId = null)
        {
            var enrollments = await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Where(e => e.Status == "Active")
                .AsNoTracking()
                .OrderBy(e => e.Student!.RegistrationNumber)
                .ToListAsync();

            var items = enrollments.Select(e => new
            {
                e.EnrollmentId,

                DisplayText =
                    $"{e.Student!.RegistrationNumber} - " +
                    $"{e.Student.FirstName} " +
                    $"{e.Student.LastName} - " +
                    $"{e.Course!.CourseCode}"
            });

            ViewBag.Enrollments = new SelectList(
                items,
                "EnrollmentId",
                "DisplayText",
                selectedEnrollmentId);
        }
    }
}