using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByStudentIdAsync(int studentId)
        {
            return await _context.Wallets
                .Include(w => w.Student)
                .Include(w => w.Transactions.OrderByDescending(t => t.CreatedDate).Take(10))
                .FirstOrDefaultAsync(w => w.StudentId == studentId);
        }

        public async Task<Wallet?> GetByIdAsync(int walletId)
        {
            return await _context.Wallets
                .Include(w => w.Student)
                .FirstOrDefaultAsync(w => w.WalletId == walletId);
        }

        public async Task<Wallet> CreateAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }

        public async Task<Wallet> UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
            return wallet;
        }
    }
}