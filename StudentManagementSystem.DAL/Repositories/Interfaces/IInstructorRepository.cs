using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IInstructorRepository
    {
        Task<List<Instructor>> GetAllInstructorsAsync();
        Task<Instructor?> GetInstructorByIdAsync(int id);
        Task<Instructor?> GetInstructorByCodeAsync(string code);
        Task<Instructor?> GetInstructorByEmailAsync(string email);
        Task<Instructor> AddInstructorAsync(Instructor instructor);
        Task<Instructor> UpdateInstructorAsync(Instructor instructor);
        Task<bool> DeleteInstructorAsync(int id);
        Task<bool> InstructorCodeExistsAsync(string code, int? excludeId = null);
        Task<bool> EmailExistsAsync(string email, int? excludeId = null);
    }
}