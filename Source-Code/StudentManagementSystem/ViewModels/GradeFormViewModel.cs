using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace StudentManagementSystem.ViewModels
{
    public class GradeFormViewModel
    {
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
        public decimal MarksObtained { get; set; }

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Total Marks")]
        public decimal TotalMarks { get; set; }

        [StringLength(250)]
        public string? Remarks { get; set; }

        public SelectList? Students { get; set; }
        public SelectList? Courses { get; set; }
    }
}