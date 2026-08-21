using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToRoleDashboard();
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string? returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email address or password.");

                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName!,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrWhiteSpace(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                return RedirectToRoleDashboard();
            }

            ModelState.AddModelError(
                string.Empty,
                "Invalid email address or password.");

            return View(model);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                return RedirectToRoleDashboard();
            }

            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var registrationExists = await _context.Students
                .AnyAsync(s =>
                    s.RegistrationNumber == model.RegistrationNumber);

            if (registrationExists)
            {
                ModelState.AddModelError(
                    nameof(model.RegistrationNumber),
                    "This registration number is already registered.");

                return View(model);
            }

            var existingUser = await _userManager
                .FindByEmailAsync(model.Email);

            if (existingUser != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists.");

                return View(model);
            }

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = $"{model.FirstName} {model.LastName}",
                PhoneNumber = model.PhoneNumber,
                EmailConfirmed = true
            };

            var createResult = await _userManager.CreateAsync(
                user,
                model.Password);

            if (!createResult.Succeeded)
            {
                foreach (var error in createResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(model);
            }

            await _userManager.AddToRoleAsync(user, "Student");

            var student = new Student
            {
                UserId = user.Id,
                RegistrationNumber = model.RegistrationNumber,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                DateOfBirth = model.DateOfBirth,
                Gender = model.Gender,
                Address = model.Address,
                CreatedAt = DateTime.Now
            };

            _context.Students.Add(student);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
                await _userManager.DeleteAsync(user);

                ModelState.AddModelError(
                    string.Empty,
                    "Registration could not be completed. Please try again.");

                return View(model);
            }

            TempData["SuccessMessage"] =
                "Registration successful. Please login with your account.";

            return RedirectToAction(nameof(Login));
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction(nameof(Login));
        }

        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }

        private IActionResult RedirectToRoleDashboard()
        {
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction(
                    "Index",
                    "Home");
            }

            if (User.IsInRole("Student"))
            {
                return RedirectToAction(
                    "Dashboard",
                    "StudentPortal");
            }

            if (User.IsInRole("Teacher"))
            {
                return RedirectToAction(
                    "Dashboard",
                    "TeacherPortal");
            }

            return RedirectToAction(nameof(Login));
        }
    }
}