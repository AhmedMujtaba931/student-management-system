using Microsoft.AspNetCore.Identity;

namespace StudentManagementSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string FullName { get; set; } = string.Empty;

        public Student? StudentProfile { get; set; }

        public Teacher? TeacherProfile { get; set; }
    }
}