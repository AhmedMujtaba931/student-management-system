# Student Management System – Database Design

## 1. Database Name

StudentManagementSystemDB

## 2. Main Database Tables

The Student Management System will use the following main tables:

1. Students
2. Courses
3. Enrollments
4. AttendanceRecords
5. Grades

## 3. Students Table

| Field Name | Description |
|---|---|
| StudentId | Unique identifier for each student |
| RegistrationNumber | Unique registration number |
| FirstName | Student's first name |
| LastName | Student's last name |
| Email | Student's email address |
| PhoneNumber | Student's phone number |
| DateOfBirth | Student's date of birth |
| Gender | Student's gender |
| Address | Student's address |
| CreatedAt | Record creation date |

## 4. Courses Table

| Field Name | Description |
|---|---|
| CourseId | Unique identifier for each course |
| CourseCode | Unique course code |
| CourseName | Name of the course |
| CreditHours | Total credit hours |
| Description | Course description |
| CreatedAt | Record creation date |

## 5. Enrollments Table

| Field Name | Description |
|---|---|
| EnrollmentId | Unique enrollment identifier |
| StudentId | Reference to the student |
| CourseId | Reference to the course |
| EnrollmentDate | Date of enrollment |
| Status | Current enrollment status |

## 6. AttendanceRecords Table

| Field Name | Description |
|---|---|
| AttendanceId | Unique attendance identifier |
| StudentId | Reference to the student |
| CourseId | Reference to the course |
| AttendanceDate | Date of attendance |
| Status | Attendance status |
| Remarks | Additional attendance remarks |

Attendance Statuses:

- Present
- Absent
- Late
- Excused

## 7. Grades Table

| Field Name | Description |
|---|---|
| GradeId | Unique grade identifier |
| StudentId | Reference to the student |
| CourseId | Reference to the course |
| AssessmentType | Type of assessment |
| MarksObtained | Marks obtained by the student |
| TotalMarks | Total marks |
| GradeLetter | Final grade |
| Remarks | Additional remarks |

## 8. Database Relationships

- One student can have multiple enrollments.
- One course can have multiple enrollments.
- One student can have multiple attendance records.
- One course can have multiple attendance records.
- One student can have multiple grade records.
- One course can have multiple grade records.

The Enrollments table will resolve the many-to-many relationship between Students and Courses.

## 9. Primary Keys and Foreign Keys

### Students

- Primary Key: `StudentId`

### Courses

- Primary Key: `CourseId`

### Enrollments

- Primary Key: `EnrollmentId`
- Foreign Key: `StudentId` → `Students.StudentId`
- Foreign Key: `CourseId` → `Courses.CourseId`

### AttendanceRecords

- Primary Key: `AttendanceId`
- Foreign Key: `StudentId` → `Students.StudentId`
- Foreign Key: `CourseId` → `Courses.CourseId`

### Grades

- Primary Key: `GradeId`
- Foreign Key: `StudentId` → `Students.StudentId`
- Foreign Key: `CourseId` → `Courses.CourseId`