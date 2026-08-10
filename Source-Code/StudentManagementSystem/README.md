# student-management-system
## Database Relationships

The Student Management System uses the following relationships:

- Students → Enrollments
  - `Enrollments.StudentId` references `Students.StudentId`

- Courses → Enrollments
  - `Enrollments.CourseId` references `Courses.CourseId`

- Students → AttendanceRecords
  - `AttendanceRecords.StudentId` references `Students.StudentId`

- Courses → AttendanceRecords
  - `AttendanceRecords.CourseId` references `Courses.CourseId`

- Students → Grades
  - `Grades.StudentId` references `Students.StudentId`

- Courses → Grades
  - `Grades.CourseId` references `Courses.CourseId`

These foreign-key relationships maintain referential integrity between students, courses, enrollments, attendance records, and grades.