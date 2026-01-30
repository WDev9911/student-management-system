using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IMajorRepository
    {
        Task<List<Major>> GetAllMajorsAsync();
        Task<Major?> GetMajorByIdAsync(int id);
        Task<Major?> GetMajorByCodeAsync(string code);
    }
}