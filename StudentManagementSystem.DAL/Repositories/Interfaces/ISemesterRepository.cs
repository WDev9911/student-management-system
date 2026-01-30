using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface ISemesterRepository
    {
        Task<List<Semester>> GetAllSemestersAsync();
        Task<Semester?> GetSemesterByIdAsync(int id);
        Task<Semester?> GetCurrentSemesterAsync();
        Task<Semester?> GetSemesterByNumberAsync(int semesterNumber);
    }
}