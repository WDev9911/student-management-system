using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class PendingEnrollmentRepository : IPendingEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public PendingEnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<PendingEnrollment>> GetPendingByStudentIdAsync(int studentId)
        {
            return await _context.PendingEnrollments
                .Include(p => p.Course)
                .Where(p => p.StudentId == studentId && p.Status == "Pending")
                .ToListAsync();
        }

        public async Task<PendingEnrollment?> GetPendingAsync(int studentId, int courseId, int semesterNumber)
        {
            return await _context.PendingEnrollments
                .FirstOrDefaultAsync(p => p.StudentId == studentId && p.CourseId == courseId && p.SemesterNumber == semesterNumber && p.Status == "Pending");
        }

        public async Task AddAsync(PendingEnrollment pending)
        {
            await _context.PendingEnrollments.AddAsync(pending);
        }

        public async Task UpdateAsync(PendingEnrollment pending)
        {
            _context.PendingEnrollments.Update(pending);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}