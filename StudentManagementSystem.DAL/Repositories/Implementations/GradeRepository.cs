using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class GradeRepository : IGradeRepository
    {
        private readonly ApplicationDbContext _context;

        public GradeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCompletedCreditsAsync(int studentId)
        {
            return await _context.Grades
                .Where(g => g.StudentId == studentId && g.IsPassed)
                .SumAsync(g => g.Course.Credits);
        }

        public async Task<List<Grade>> GetByEnrollmentIdsAsync(List<int> enrollmentIds)
        {
            return await _context.Grades
                .Where(g => enrollmentIds.Contains(g.EnrollmentId))
                .ToListAsync();
        }

        public async Task UpsertGradesAsync(List<Grade> grades)
        {
            var toAdd = grades.Where(g => g.GradeId == 0).ToList();
            var toUpdate = grades.Where(g => g.GradeId != 0).ToList();

            if (toAdd.Any()) await _context.Grades.AddRangeAsync(toAdd);
            if (toUpdate.Any()) _context.Grades.UpdateRange(toUpdate);

            await _context.SaveChangesAsync();
        }

        public async Task<List<Grade>> GetPublishedGradesByStudentIdAsync(int studentId)
        {
            return await _context.Grades
                .Include(g => g.Course)
                .Include(g => g.Enrollment)
                .Where(g => g.StudentId == studentId && g.IsPublished)
                .OrderBy(g => g.Enrollment.SemesterNumber)
                .ThenBy(g => g.Course.CourseCode)
                .ToListAsync();
        }
    }
}