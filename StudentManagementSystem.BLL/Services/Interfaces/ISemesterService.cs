using StudentManagementSystem.BLL.DTOs;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface ISemesterService
    {
        Task<List<SemesterDto>> GetAllSemestersAsync();
        Task<SemesterDto?> GetSemesterByIdAsync(int id);
        Task<SemesterDto?> GetCurrentSemesterAsync();
        Task<(bool Success, string Message)> UpdateSemesterDatesAsync(int semesterId, DateTime startDate, DateTime endDate);
    }
}