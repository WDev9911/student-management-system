using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class RegistrationRequestRepository : IRegistrationRequestRepository
    {
        private readonly ApplicationDbContext _db;

        public RegistrationRequestRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public Task<IEnumerable<RegistrationRequest>> GetPendingRequestsAsync()
        {
            return Task.FromResult<IEnumerable<RegistrationRequest>>(
                _db.RegistrationRequests
                    .Where(r => r.Status == "Pending")
                    .OrderBy(r => r.RequestDate)
                    .AsEnumerable()
            );
        }

        public Task<RegistrationRequest?> GetByIdAsync(int requestId)
        {
            return _db.RegistrationRequests.FirstOrDefaultAsync(r => r.RequestId == requestId);
        }

        public async Task CreateAsync(RegistrationRequest request)
        {
            await _db.RegistrationRequests.AddAsync(request);
            await _db.SaveChangesAsync();
        }

        public async Task UpdateAsync(RegistrationRequest request)
        {
            _db.RegistrationRequests.Update(request);
            await _db.SaveChangesAsync();
        }

        public Task<bool> IsUsernameExistsAsync(string username)
        {
            return _db.Users.AnyAsync(u => u.Username == username);
        }

        public Task<bool> IsEmailExistsInPendingAsync(string email)
        {
            return _db.RegistrationRequests.AnyAsync(r => r.Email == email && r.Status == "Pending");
        }
    }
}