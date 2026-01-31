using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetByIdAsync(int enrollmentId);
        Task<List<Enrollment>> GetActiveEnrollmentsByStudentIdAsync(int studentId);
        Task<List<Enrollment>> GetEnrollmentsByClassIdAsync(int classId);
        Task<bool> IsEnrolledInClassAsync(int studentId, int classId);
        Task<bool> HasScheduleConflictAsync(int studentId, int classId);
        Task CreateEnrollmentAsync(Enrollment enrollment);
        Task UpdateEnrollmentAsync(Enrollment enrollment);
        Task SaveChangesAsync();
    }
}