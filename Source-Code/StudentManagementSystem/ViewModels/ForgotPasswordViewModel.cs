using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Required]
        [Display(Name = "Registration Number (Student) / Employee Code (Teacher)")]
        public string IdentifierCode { get; set; } = string.Empty;
    }
}