using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IWalletRepository
    {
        Task<Wallet?> GetByStudentIdAsync(int studentId);
        Task<Wallet?> GetByIdAsync(int walletId);
        Task<Wallet> CreateAsync(Wallet wallet);
        Task<Wallet> UpdateAsync(Wallet wallet);
        Task<bool> SaveChangesAsync();
    }
}