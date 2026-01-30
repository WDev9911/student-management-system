using StudentManagementSystem.BLL.DTOs;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface ICourseService
    {
        Task<List<CourseDto>> GetAllCoursesAsync();
        Task<CourseDto?> GetCourseByIdAsync(int id);
        Task<List<CourseDto>> GetCoursesByMajorAndSemesterAsync(int majorId, int semesterNumber);
    }
}