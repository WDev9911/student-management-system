using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IRegistrationRequestRepository
    {
        Task<IEnumerable<RegistrationRequest>> GetPendingRequestsAsync();
        Task<RegistrationRequest?> GetByIdAsync(int requestId);
        Task CreateAsync(RegistrationRequest request);
        Task UpdateAsync(RegistrationRequest request);
        Task<bool> IsUsernameExistsAsync(string username);
        Task<bool> IsEmailExistsInPendingAsync(string email);
    }
}