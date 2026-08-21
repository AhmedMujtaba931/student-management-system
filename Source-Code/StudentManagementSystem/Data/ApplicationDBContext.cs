using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<AttendanceSession> AttendanceSessions { get; set; }
        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }
        public DbSet<Grade> Grades { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.RegistrationNumber).IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.UserId).IsUnique()
                .HasFilter("[UserId] IS NOT NULL");

            modelBuilder.Entity<Student>()
                .HasOne(s => s.User).WithOne(u => u.StudentProfile)
                .HasForeignKey<Student>(s => s.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.EmployeeCode).IsUnique();

            modelBuilder.Entity<Teacher>()
                .HasIndex(t => t.UserId).IsUnique()
                .HasFilter("[UserId] IS NOT NULL");

            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.User).WithOne(u => u.TeacherProfile)
                .HasForeignKey<Teacher>(t => t.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Course>()
                .HasIndex(c => c.CourseCode).IsUnique();

            modelBuilder.Entity<Course>()
                .HasOne(c => c.Teacher).WithMany(t => t.Courses)
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student).WithMany()
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course).WithMany()
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Grade>()
                .Property(g => g.MarksObtained).HasPrecision(10, 2);

            modelBuilder.Entity<Grade>()
                .Property(g => g.TotalMarks).HasPrecision(10, 2);
        }
    }
}