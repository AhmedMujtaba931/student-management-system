using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.ViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [StringLength(20)]
        [Display(Name = "Registration Number")]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [StringLength(100)]
        [Display(Name = "Email Address")]
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Address { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 8)]
        [Display(Name = "Password")]
        public string Password { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password))]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}