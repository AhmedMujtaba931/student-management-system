using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                    return RedirectToAction("Index", "Admin");
                if (User.IsInRole("Teacher"))
                    return RedirectToAction("Dashboard", "TeacherPortal");
                if (User.IsInRole("Student"))
                    return RedirectToAction("Dashboard", "StudentPortal");
            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}