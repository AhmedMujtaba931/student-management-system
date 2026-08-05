using System.ComponentModel.DataAnnotations;

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
        [Range(0, 100)]
        [Display(Name = "Marks Obtained")]
        public decimal MarksObtained { get; set; }

        [Required]
        [Range(1, 100)]
        [Display(Name = "Total Marks")]
        public decimal TotalMarks { get; set; }

        [StringLength(5)]
        [Display(Name = "Grade")]
        public string? GradeLetter { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        // Navigation Properties
        public Student? Student { get; set; }

        public Course? Course { get; set; }
    }
}