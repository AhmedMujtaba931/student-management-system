using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }

        [StringLength(450)]
        [Display(Name = "User Account")]
        public string? UserId { get; set; }

        [Required]
        [StringLength(20)]
        [Display(Name = "Employee Code")]
        public string EmployeeCode { get; set; } = string.Empty;

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
        public string Email { get; set; } = string.Empty;

        [Phone]
        [StringLength(20)]
        [Display(Name = "Phone Number")]
        public string? PhoneNumber { get; set; }

        [Required]
        [StringLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Designation { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        [Display(Name = "Date of Birth")]
        public DateTime? DateOfBirth { get; set; }

        [Required]
        [StringLength(20)]
        public string Gender { get; set; } = string.Empty;

        [StringLength(250)]
        public string? Address { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ApplicationUser? User { get; set; }
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}