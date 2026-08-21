using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.Data;
using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public class GradeService : IGradeService
    {
        private readonly ApplicationDbContext _context;

        public GradeService(ApplicationDbContext context)
        {
            _context = context;
        }

        public string CalculateLetter(decimal marksObtained, decimal totalMarks)
        {
            if (totalMarks <= 0) return "N/A";

            var pct = (marksObtained / totalMarks) * 100m;

            return pct switch
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
                >= 40 => "D",
                _ => "F"
            };
        }

        public async Task<IReadOnlyList<Grade>> GetAllAsync()
        {
            return await _context.Grades
                .AsNoTracking()
                .Include(g => g.Student)
                .Include(g => g.Course)
                .OrderByDescending(g => g.GradeId)
                .ToListAsync();
        }

        public async Task<Grade?> GetByIdAsync(int id)
        {
            return await _context.Grades
                .Include(g => g.Student)
                .Include(g => g.Course)
                .FirstOrDefaultAsync(g => g.GradeId == id);
        }

        public async Task<Grade> CreateAsync(Grade grade)
        {
            grade.GradeLetter = CalculateLetter(grade.MarksObtained, grade.TotalMarks);
            grade.CreatedAt = DateTime.UtcNow;
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
            return grade;
        }

        public async Task UpdateAsync(Grade grade)
        {
            grade.GradeLetter = CalculateLetter(grade.MarksObtained, grade.TotalMarks);
            _context.Grades.Update(grade);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var grade = await _context.Grades.FindAsync(id);
            if (grade != null)
            {
                _context.Grades.Remove(grade);
                await _context.SaveChangesAsync();
            }
        }
    }
}