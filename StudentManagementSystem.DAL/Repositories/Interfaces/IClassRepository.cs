using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IClassRepository
    {
        Task<List<CourseClass>> GetAllAsync();
        Task<CourseClass?> GetByIdAsync(int id);
        Task<List<CourseClass>> GetByCourseAndSemesterAsync(int courseId, int semesterId);
        Task<List<CourseClass>> GetByInstructorAsync(int instructorId);
        Task<List<CourseClass>> GetByInstructorAndSemesterAsync(int instructorId, int semesterId);
        Task<List<CourseClass>> GetByRoomAndSemesterAsync(string room, int semesterId);
        Task<bool> ClassNameExistsAsync(string className, int? excludeId = null);
        Task<CourseClass> CreateAsync(CourseClass courseClass);
        Task<CourseClass> UpdateAsync(CourseClass courseClass);
        Task<bool> DeleteAsync(int id);
        Task<CourseClass?> GetClassByIdAsync(int classId);
        Task<List<CourseClass>> GetClassesByCourseAsync(int courseId);
        Task<bool> SaveChangesAsync();
    }
}