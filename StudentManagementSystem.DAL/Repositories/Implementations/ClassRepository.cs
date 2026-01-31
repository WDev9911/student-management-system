using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class ClassRepository : IClassRepository
    {
        private readonly ApplicationDbContext _context;

        public ClassRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<CourseClass>> GetAllAsync()
        {
            return await _context.CourseClasses
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester)
                .OrderBy(c => c.ClassName)
                .ToListAsync();
        }

        public async Task<CourseClass?> GetByIdAsync(int id)
        {
            return await _context.CourseClasses
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester)
                .FirstOrDefaultAsync(c => c.ClassId == id);
        }

        public async Task<List<CourseClass>> GetByCourseAndSemesterAsync(int courseId, int semesterId)
        {
            return await _context.CourseClasses
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester)
                .Where(c => c.CourseId == courseId && c.SemesterId == semesterId)
                .OrderBy(c => c.ClassName)
                .ToListAsync();
        }

        public async Task<List<CourseClass>> GetByInstructorAsync(int instructorId)
        {
            return await _context.CourseClasses
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester)
                .Where(c => c.InstructorId == instructorId)
                .OrderBy(c => c.Semester.SemesterNumber)
                .ThenBy(c => c.ClassName)
                .ToListAsync();
        }

        public async Task<List<CourseClass>> GetByInstructorAndSemesterAsync(int instructorId, int semesterId)
        {
            return await _context.CourseClasses
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester)
                .Where(c => c.InstructorId == instructorId && c.SemesterId == semesterId)
                .ToListAsync();
        }

        public async Task<List<CourseClass>> GetByRoomAndSemesterAsync(string room, int semesterId)
        {
            return await _context.CourseClasses
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester)
                .Where(c => c.Room == room && c.SemesterId == semesterId)
                .ToListAsync();
        }

        public async Task<bool> ClassNameExistsAsync(string className, int? excludeId = null)
        {
            return await _context.CourseClasses
                .AnyAsync(c => c.ClassName == className && (excludeId == null || c.ClassId != excludeId));
        }

        public async Task<CourseClass> CreateAsync(CourseClass courseClass)
        {
            _context.CourseClasses.Add(courseClass);
            await _context.SaveChangesAsync();
            return courseClass;
        }

        public async Task<CourseClass> UpdateAsync(CourseClass courseClass)
        {
            _context.CourseClasses.Update(courseClass);
            await _context.SaveChangesAsync();
            return courseClass;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var courseClass = await GetByIdAsync(id);
            if (courseClass == null) return false;

            _context.CourseClasses.Remove(courseClass);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<CourseClass?> GetClassByIdAsync(int classId)
        {
            return await _context.CourseClasses
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester)
                .FirstOrDefaultAsync(c => c.ClassId == classId);
        }

        public async Task<List<CourseClass>> GetClassesByCourseAsync(int courseId)
        {
            return await _context.CourseClasses
                .Include(c => c.Course)
                .Include(c => c.Instructor)
                .Include(c => c.Semester)
                .Where(c => c.CourseId == courseId && c.Status == "Open")
                .ToListAsync();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}