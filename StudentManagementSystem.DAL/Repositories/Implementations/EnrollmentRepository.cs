using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly ApplicationDbContext _context;

        public EnrollmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Enrollment?> GetByIdAsync(int enrollmentId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.CourseClass)
                    .ThenInclude(c => c.Instructor)
                .FirstOrDefaultAsync(e => e.EnrollmentId == enrollmentId);
        }

        public async Task<List<Enrollment>> GetActiveEnrollmentsByStudentIdAsync(int studentId)
        {
            return await _context.Enrollments
                .Include(e => e.Course)
                .Include(e => e.CourseClass)
                    .ThenInclude(c => c.Instructor)
                .Where(e => e.StudentId == studentId && e.Status == "Active")
                .ToListAsync();
        }

        public async Task<bool> IsEnrolledInClassAsync(int studentId, int classId)
        {
            return await _context.Enrollments
                .AnyAsync(e => e.StudentId == studentId && e.ClassId == classId && e.Status == "Active");
        }

        public async Task<bool> HasScheduleConflictAsync(int studentId, int classId)
        {
            var newClass = await _context.CourseClasses
                .FirstOrDefaultAsync(c => c.ClassId == classId);

            if (newClass == null) return false;

            var studentEnrollments = await _context.Enrollments
                .Include(e => e.CourseClass)
                .Where(e => e.StudentId == studentId && e.Status == "Active")
                .ToListAsync();

            foreach (var enrollment in studentEnrollments)
            {
                var existingClass = enrollment.CourseClass;

                var hasDates = existingClass.StartDate.HasValue &&
                               existingClass.EndDate.HasValue &&
                               newClass.StartDate.HasValue &&
                               newClass.EndDate.HasValue;

                if (hasDates && !HasDateOverlap(existingClass.StartDate, existingClass.EndDate,
                                                newClass.StartDate, newClass.EndDate))
                {
                    continue;
                }

                if (HasScheduleOverlap(existingClass.DayOfWeek, existingClass.StartTime, existingClass.EndTime,
                                       newClass.DayOfWeek, newClass.StartTime, newClass.EndTime))
                {
                    return true;
                }
            }

            return false;
        }

        private static bool HasScheduleOverlap(string days1, TimeSpan start1, TimeSpan end1,
                                               string days2, TimeSpan start2, TimeSpan end2)
        {
            var daySet1 = days1.Split(',').Select(d => d.Trim()).ToHashSet();
            var daySet2 = days2.Split(',').Select(d => d.Trim()).ToHashSet();

            if (!daySet1.Intersect(daySet2).Any()) return false;

            return start1 < end2 && start2 < end1;
        }

        private static bool HasDateOverlap(DateTime? start1, DateTime? end1,
                                           DateTime? start2, DateTime? end2)
        {
            if (!start1.HasValue || !end1.HasValue || !start2.HasValue || !end2.HasValue)
            {
                return true;
            }

            return start1.Value.Date <= end2.Value.Date && start2.Value.Date <= end1.Value.Date;
        }

        public async Task CreateEnrollmentAsync(Enrollment enrollment)
        {
            await _context.Enrollments.AddAsync(enrollment);
        }

        public async Task UpdateEnrollmentAsync(Enrollment enrollment)
        {
            _context.Enrollments.Update(enrollment);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Enrollment>> GetEnrollmentsByClassIdAsync(int classId)
        {
            return await _context.Enrollments
                .Include(e => e.Student)
                .Include(e => e.Course)
                .Include(e => e.CourseClass)
                .Where(e => e.ClassId == classId)
                .OrderBy(e => e.Student.StudentCode)
                .ToListAsync();
        }
    }
}