using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceId { get; set; }

        [Required]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Attendance Date")]
        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Present";

        [StringLength(250)]
        public string? Remarks { get; set; }

        // Navigation Properties
        public Student? Student { get; set; }

        public Course? Course { get; set; }
    }
}