using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Student
        public async Task<IActionResult> Index()
        {
            var students = await _context.Students
                .AsNoTracking()
                .OrderBy(s => s.RegistrationNumber)
                .ToListAsync();

            return View(students);
        }

        // GET: /Student/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // GET: /Student/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: /Student/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Student student)
        {
            // Prevent duplicate registration numbers.
            bool registrationExists = await _context.Students
                .AnyAsync(s => s.RegistrationNumber == student.RegistrationNumber);

            if (registrationExists)
            {
                ModelState.AddModelError(
                    nameof(student.RegistrationNumber),
                    "This registration number already exists."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(student);
            }

            student.CreatedAt = DateTime.Now;

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Student added successfully.";

            return RedirectToAction(nameof(Index));
        }

        // GET: /Student/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: /Student/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Student student)
        {
            if (id != student.StudentId)
            {
                return NotFound();
            }

            bool registrationExists = await _context.Students
                .AnyAsync(s =>
                    s.RegistrationNumber == student.RegistrationNumber &&
                    s.StudentId != student.StudentId);

            if (registrationExists)
            {
                ModelState.AddModelError(
                    nameof(student.RegistrationNumber),
                    "This registration number already belongs to another student."
                );
            }

            if (!ModelState.IsValid)
            {
                return View(student);
            }

            try
            {
                var existingStudent = await _context.Students
                    .FindAsync(id);

                if (existingStudent == null)
                {
                    return NotFound();
                }

                existingStudent.RegistrationNumber = student.RegistrationNumber;
                existingStudent.FirstName = student.FirstName;
                existingStudent.LastName = student.LastName;
                existingStudent.Email = student.Email;
                existingStudent.PhoneNumber = student.PhoneNumber;
                existingStudent.DateOfBirth = student.DateOfBirth;
                existingStudent.Gender = student.Gender;
                existingStudent.Address = student.Address;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Student updated successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await StudentExists(student.StudentId))
                {
                    return NotFound();
                }

                throw;
            }
        }

        // GET: /Student/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var student = await _context.Students
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StudentId == id);

            if (student == null)
            {
                return NotFound();
            }

            return View(student);
        }

        // POST: /Student/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound();
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Student deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> StudentExists(int id)
        {
            return await _context.Students
                .AnyAsync(s => s.StudentId == id);
        }
    }
}