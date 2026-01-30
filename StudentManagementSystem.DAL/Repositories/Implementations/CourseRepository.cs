using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class CourseRepository : ICourseRepository
    {
        private readonly ApplicationDbContext _context;

        public CourseRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Course>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Include(c => c.Major)
                .Include(c => c.PrerequisiteCourse)
                .OrderBy(c => c.MajorId)
                .ThenBy(c => c.SemesterNumber)
                .ThenBy(c => c.CourseCode)
                .ToListAsync();
        }

        public async Task<Course?> GetCourseByIdAsync(int id)
        {
            return await _context.Courses
                .Include(c => c.Major)
                .Include(c => c.PrerequisiteCourse)
                .FirstOrDefaultAsync(c => c.CourseId == id);
        }

        public async Task<List<Course>> GetCoursesByMajorAndSemesterAsync(int majorId, int semesterNumber)
        {
            return await _context.Courses
                .Include(c => c.Major)
                .Include(c => c.PrerequisiteCourse)
                .Where(c => c.MajorId == majorId && c.SemesterNumber == semesterNumber)
                .OrderBy(c => c.CourseCode)
                .ToListAsync();
        }
    }
}