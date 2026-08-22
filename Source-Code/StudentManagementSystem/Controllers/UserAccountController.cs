using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Models;
using StudentManagementSystem.Services;
using StudentManagementSystem.ViewModels;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserAccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserAccountController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.AsNoTracking().ToListAsync();
            var list = new List<UserAccountListItem>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                list.Add(new UserAccountListItem
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    Role = roles.FirstOrDefault() ?? "None"
                });
            }

            var ordered = list.OrderBy(x => x.Role).ThenBy(x => x.FullName).ToList();
            return View(ordered);
        }

        public async Task<IActionResult> ResetPasswordConfirm(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            ViewBag.Email = user.Email;
            ViewBag.FullName = user.FullName;
            ViewBag.Role = roles.FirstOrDefault() ?? "None";
            ViewBag.UserId = user.Id;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound();

            var newPassword = PasswordGenerator.Generate();
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, newPassword);

            if (!result.Succeeded)
            {
                TempData["ErrorMessage"] = "Password reset failed. Please try again.";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Email = user.Email;
            ViewBag.FullName = user.FullName;
            ViewBag.NewPassword = newPassword;

            return View("PasswordResetResult");
        }
    }
}