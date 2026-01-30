using StudentManagementSystem.BLL.DTOs;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface IInstructorService
    {
        Task<(bool Success, string Message)> CreateInstructorAsync(CreateInstructorDto dto);
        Task<IEnumerable<InstructorListDto>> GetAllInstructorsAsync();
        Task<InstructorDetailDto?> GetInstructorByIdAsync(int instructorId);
        Task<(bool Success, string Message)> UpdateInstructorAsync(int instructorId, CreateInstructorDto dto);
        Task<(bool Success, string Message)> DeleteInstructorAsync(int instructorId);
    }
}