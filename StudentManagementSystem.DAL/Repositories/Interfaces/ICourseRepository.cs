using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllCoursesAsync();
        Task<Course?> GetCourseByIdAsync(int id);
        Task<List<Course>> GetCoursesByMajorAndSemesterAsync(int majorId, int semesterNumber);
        Task<List<Course>> GetCoursesByMajorAsync(int majorId);
    }
}