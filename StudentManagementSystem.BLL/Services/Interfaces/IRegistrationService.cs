using StudentManagementSystem.BLL.DTOs;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface IRegistrationService
    {
        Task<(bool Success, string Message)> RegisterAsync(RegisterRequestDto dto);
        Task<IEnumerable<RegistrationRequestListDto>> GetPendingRequestsAsync();
        Task<(bool Success, string Message)> ApproveRequestAsync(int requestId, int adminUserId);
        Task<(bool Success, string Message)> RejectRequestAsync(int requestId, int adminUserId, string reason);
        Task<IEnumerable<MajorDto>> GetMajorsAsync();
    }
}