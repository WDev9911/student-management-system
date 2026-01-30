using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface ITransactionRepository
    {
        Task<List<Transaction>> GetByStudentIdAsync(int studentId);
        Task<Transaction?> GetByIdAsync(int transactionId);
        Task<Transaction?> GetByTransactionCodeAsync(string transactionCode);
        Task<Transaction> CreateAsync(Transaction transaction);
        Task<Transaction> UpdateAsync(Transaction transaction);
        Task<List<Transaction>> GetRecentTransactionsAsync(int studentId, int count = 10);
    }
}