using StudentManagementSystem.BLL.DTOs;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface IMajorService
    {
        Task<List<MajorDto>> GetAllMajorsAsync();
        Task<MajorDto?> GetMajorByIdAsync(int id);
    }
}