namespace StudentManagementSystem.ViewModels
{
    public class MarkAttendanceViewModel
    {
        public int CourseId { get; set; }
        public DateTime AttendanceDate { get; set; } = DateTime.Today;
        public List<AttendanceEntry> Entries { get; set; } = new();
    }

    public class AttendanceEntry
    {
        public int EnrollmentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public string Status { get; set; } = "Present";
    }
}