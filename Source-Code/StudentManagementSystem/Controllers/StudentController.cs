using Microsoft.AspNetCore.Mvc;

namespace StudentManagementSystem.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}