using StudentManagementSystem.Models;

namespace StudentManagementSystem.Services
{
    public interface IGradeService
    {
        string CalculateLetter(decimal marksObtained, decimal totalMarks);
        Task<IReadOnlyList<Grade>> GetAllAsync();
        Task<Grade?> GetByIdAsync(int id);
        Task<Grade> CreateAsync(Grade grade);
        Task UpdateAsync(Grade grade);
        Task DeleteAsync(int id);
    }
}