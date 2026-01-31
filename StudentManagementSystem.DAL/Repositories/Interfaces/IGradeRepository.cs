using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IGradeRepository
    {
        Task<int> GetCompletedCreditsAsync(int studentId);
        Task<List<Grade>> GetByEnrollmentIdsAsync(List<int> enrollmentIds);
        Task UpsertGradesAsync(List<Grade> grades);
        Task<List<Grade>> GetPublishedGradesByStudentIdAsync(int studentId);
    }
}