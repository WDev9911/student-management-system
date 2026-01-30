using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IStudentRepository
    {
        Task<Student> CreateAsync(Student student);
        Task<string> GenerateStudentCodeAsync();
        Task<Student?> GetByIdWithDetailsAsync(int studentId);
        Task<decimal> GetWalletBalanceAsync(int studentId);
        Task<int> GetEnrolledCoursesCountAsync(int studentId);
        Task<List<Enrollment>> GetActiveEnrollmentsAsync(int studentId);
    }
}