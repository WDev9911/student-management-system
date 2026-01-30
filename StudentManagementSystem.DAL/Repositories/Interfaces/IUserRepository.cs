using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task UpdateLastLoginAsync(int userId);
        Task CreateUserAsync(User user);
        Task CreateWalletAsync(Wallet wallet);
    }
}