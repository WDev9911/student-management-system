using StudentManagementSystem.BLL.DTOs;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface IStudentService
    {
        Task<StudentDashboardDto?> GetDashboardDataAsync(int studentId);
        Task<WalletInfoDto?> GetWalletInfoAsync(int studentId);
    }
}