using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;

        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<User?> GetByUsernameAsync(string username)
        {
            return _db.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task UpdateLastLoginAsync(int userId)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserId == userId);
            if (user == null)
            {
                return;
            }

            user.LastLoginDate = DateTime.Now;
            await _db.SaveChangesAsync();
        }

        public async Task CreateUserAsync(User user)
        {
            await _db.Users.AddAsync(user);
            await _db.SaveChangesAsync();
        }

        public async Task CreateWalletAsync(Wallet wallet)
        {
            await _db.Wallets.AddAsync(wallet);
            await _db.SaveChangesAsync();
        }
    }
}