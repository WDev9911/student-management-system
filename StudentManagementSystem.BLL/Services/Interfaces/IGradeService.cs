using StudentManagementSystem.BLL.DTOs;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface IGradeService
    {
        Task<GradeEntryDto?> GetGradeEntryAsync(int classId, int instructorId);
        Task SaveGradesAsync(GradeEntryDto model, string action);
    }
}