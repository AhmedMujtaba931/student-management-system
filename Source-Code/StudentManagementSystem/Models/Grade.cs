using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.Models
{
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }

        [Required]
        [Display(Name = "Student")]
        public int StudentId { get; set; }

        [Required]
        [Display(Name = "Course")]
        public int CourseId { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Assessment Type")]
        public string AssessmentType { get; set; } = string.Empty;

        [Required]
        [Range(0, 1000)]
        [Display(Name = "Marks Obtained")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal MarksObtained { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Total Marks")]
        [Column(TypeName = "decimal(10,2)")]
        public decimal TotalMarks { get; set; }

        [StringLength(5)]
        [Display(Name = "Grade Letter")]
        public string? GradeLetter { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Student? Student { get; set; }
        public Course? Course { get; set; }

        [NotMapped]
        public decimal Percentage =>
            TotalMarks > 0
                ? Math.Round((MarksObtained / TotalMarks) * 100, 2)
                : 0;
    }
}