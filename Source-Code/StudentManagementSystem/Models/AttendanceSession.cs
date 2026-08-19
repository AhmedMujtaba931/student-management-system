using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Models
{
    public class AttendanceSession
    {
        [Key]
        public int AttendanceSessionId { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        [Display(Name = "Lecture Number")]
        public int LectureNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Lecture Date")]
        public DateTime LectureDate { get; set; } = DateTime.Today;

        [Required]
        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(200)]
        [Display(Name = "Topic / Description")]
        public string Topic { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public Course? Course { get; set; }

        public ICollection<AttendanceRecord> AttendanceRecords { get; set; }
            = new List<AttendanceRecord>();
    }
}