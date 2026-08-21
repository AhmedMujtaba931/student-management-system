using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceRecordId { get; set; }

        [Required]
        [Display(Name = "Enrollment")]
        public int EnrollmentId { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Attendance Date")]
        public DateTime AttendanceDate { get; set; } = DateTime.Today;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Present";

        // Navigation property
        public Enrollment? Enrollment { get; set; }
    }
}