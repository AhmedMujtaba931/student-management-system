using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudentManagementSystem.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentPortalController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}