using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IPendingEnrollmentRepository
    {
        Task<List<PendingEnrollment>> GetPendingByStudentIdAsync(int studentId);
        Task<PendingEnrollment?> GetPendingAsync(int studentId, int courseId, int semesterNumber);
        Task AddAsync(PendingEnrollment pending);
        Task UpdateAsync(PendingEnrollment pending);
        Task SaveChangesAsync();
    }
}