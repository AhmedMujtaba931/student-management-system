using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceRecordId { get; set; }

        [Required]
        public int AttendanceSessionId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Present";

        [StringLength(250)]
        public string? Remarks { get; set; }

        // Navigation properties
        public AttendanceSession? AttendanceSession { get; set; }

        public Student? Student { get; set; }
    }
}