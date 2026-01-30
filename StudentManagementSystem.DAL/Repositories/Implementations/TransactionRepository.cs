using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class TransactionRepository : ITransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public TransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Transaction>> GetByStudentIdAsync(int studentId)
        {
            return await _context.Transactions
                .Where(t => t.StudentId == studentId)
                .OrderByDescending(t => t.CreatedDate)
                .ToListAsync();
        }

        public async Task<Transaction?> GetByIdAsync(int transactionId)
        {
            return await _context.Transactions.FindAsync(transactionId);
        }

        public async Task<Transaction?> GetByTransactionCodeAsync(string transactionCode)
        {
            return await _context.Transactions
                .FirstOrDefaultAsync(t => t.TransactionCode == transactionCode);
        }

        public async Task<Transaction> CreateAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<Transaction> UpdateAsync(Transaction transaction)
        {
            _context.Transactions.Update(transaction);
            await _context.SaveChangesAsync();
            return transaction;
        }

        public async Task<List<Transaction>> GetRecentTransactionsAsync(int studentId, int count = 10)
        {
            return await _context.Transactions
                .Where(t => t.StudentId == studentId)
                .OrderByDescending(t => t.CreatedDate)
                .Take(count)
                .ToListAsync();
        }
    }
}