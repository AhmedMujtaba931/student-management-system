using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public TeacherController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var teachers = await _context.Teachers
                .AsNoTracking()
                .OrderBy(t => t.EmployeeCode)
                .ToListAsync();

            return View(teachers);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers
                .AsNoTracking()
                .Include(t => t.Courses)
                .FirstOrDefaultAsync(t => t.TeacherId == id);

            if (teacher == null) return NotFound();

            return View(teacher);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTeacherViewModel model)
        {
            bool employeeCodeExists = await _context.Teachers.AnyAsync(t => t.EmployeeCode == model.EmployeeCode);
            if (employeeCodeExists)
                ModelState.AddModelError(nameof(model.EmployeeCode), "This employee code already exists.");

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
                ModelState.AddModelError(nameof(model.Email), "An account with this email already exists.");

            if (!ModelState.IsValid) return View(model);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = $"{model.FirstName} {model.LastName}",
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(user, model.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);

                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "Teacher");

            var teacher = new Teacher
            {
                UserId = user.Id,
                EmployeeCode = model.EmployeeCode,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                Department = model.Department,
                Designation = model.Designation,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address,
                CreatedAt = DateTime.Now
            };

            _context.Teachers.Add(teacher);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                ModelState.AddModelError(string.Empty, "Teacher account could not be created. Please try again.");
                return View(model);
            }

            TempData["SuccessMessage"] = $"Teacher account created. Login email: {model.Email}";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            return View(teacher);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Teacher teacher)
        {
            if (id != teacher.TeacherId) return NotFound();

            bool employeeCodeExists = await _context.Teachers
                .AnyAsync(t => t.EmployeeCode == teacher.EmployeeCode && t.TeacherId != teacher.TeacherId);

            if (employeeCodeExists)
                ModelState.AddModelError(nameof(teacher.EmployeeCode), "This employee code already belongs to another teacher.");

            if (!ModelState.IsValid) return View(teacher);

            try
            {
                var existingTeacher = await _context.Teachers.FindAsync(id);
                if (existingTeacher == null) return NotFound();

                existingTeacher.EmployeeCode = teacher.EmployeeCode;
                existingTeacher.FirstName = teacher.FirstName;
                existingTeacher.LastName = teacher.LastName;
                existingTeacher.Email = teacher.Email;
                existingTeacher.PhoneNumber = teacher.PhoneNumber;
                existingTeacher.Department = teacher.Department;
                existingTeacher.Designation = teacher.Designation;
                existingTeacher.DateOfBirth = teacher.DateOfBirth;
                existingTeacher.Gender = teacher.Gender;
                existingTeacher.Address = teacher.Address;

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Teacher updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Teachers.AnyAsync(t => t.TeacherId == teacher.TeacherId)) return NotFound();
                throw;
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var teacher = await _context.Teachers.AsNoTracking().FirstOrDefaultAsync(t => t.TeacherId == id);
            if (teacher == null) return NotFound();

            return View(teacher);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var teacher = await _context.Teachers.FindAsync(id);
            if (teacher == null) return NotFound();

            var assignedCourses = await _context.Courses.Where(c => c.TeacherId == teacher.TeacherId).ToListAsync();
            foreach (var course in assignedCourses) course.TeacherId = null;

            var userId = teacher.UserId;

            _context.Teachers.Remove(teacher);
            await _context.SaveChangesAsync();

            if (!string.IsNullOrWhiteSpace(userId))
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null) await _userManager.DeleteAsync(user);
            }

            TempData["SuccessMessage"] = "Teacher deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
    }
}