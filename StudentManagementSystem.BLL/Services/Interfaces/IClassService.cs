using StudentManagementSystem.BLL.DTOs;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface IClassService
    {
        Task<List<ClassDto>> GetAllClassesAsync();
        Task<ClassDto?> GetClassByIdAsync(int id);
        Task<List<ClassDto>> GetClassesByCourseAndSemesterAsync(int courseId, int semesterId);
        Task<List<ClassDto>> GetClassesByInstructorAsync(int instructorId);
        Task<ClassStudentsDto?> GetClassStudentsAsync(int classId, int instructorId);
        Task<ClassDto> CreateClassAsync(CreateClassRequest request);
        Task<ClassDto> UpdateClassAsync(int id, UpdateClassRequest request);
        Task<bool> DeleteClassAsync(int id);
        Task<ConflictCheckResult> CheckConflictsAsync(ConflictCheckRequest request);
    }
}