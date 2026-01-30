using StudentManagementSystem.BLL.DTOs;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface IAuthService
    {
        Task<AuthUserDto?> AuthenticateAsync(string username, string password);
    }
}