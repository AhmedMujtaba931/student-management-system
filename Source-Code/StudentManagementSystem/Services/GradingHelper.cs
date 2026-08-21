namespace StudentManagementSystem.Services
{
    public static class GradingHelper
    {
        public static readonly string[] AssessmentTypes =
        {
            "Quiz", "Assignment", "Midterm", "Final", "Project"
        };

        public static double CalculatePercentage(decimal marksObtained, decimal totalMarks)
        {
            if (totalMarks <= 0) return 0;
            return (double)Math.Round((marksObtained / totalMarks) * 100m, 2);
        }

        public static string CalculateGradeLetter(decimal marksObtained, decimal totalMarks)
        {
            var percentage = CalculatePercentage(marksObtained, totalMarks);
            return percentage switch
            {
                >= 90 => "A+",
                >= 85 => "A",
                >= 80 => "A-",
                >= 75 => "B+",
                >= 70 => "B",
                >= 65 => "B-",
                >= 60 => "C+",
                >= 55 => "C",
                >= 50 => "C-",
                >= 45 => "D",
                _ => "F"
            };
        }
    }
}