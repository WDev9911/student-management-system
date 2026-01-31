using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly ApplicationDbContext _context;

        public AttendanceRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<AttendanceSession?> GetSessionWithRecordsAsync(int classId, DateTime sessionDate)
        {
            return await _context.AttendanceSessions
                .Include(s => s.Records)
                .ThenInclude(r => r.Student)
                .FirstOrDefaultAsync(s => s.ClassId == classId && s.SessionDate == sessionDate.Date);
        }

        public async Task<AttendanceSession?> GetSessionAsync(int classId, DateTime sessionDate)
        {
            return await _context.AttendanceSessions
                .FirstOrDefaultAsync(s => s.ClassId == classId && s.SessionDate == sessionDate.Date);
        }

        public async Task CreateSessionAsync(AttendanceSession session)
        {
            await _context.AttendanceSessions.AddAsync(session);
        }

        public async Task<List<AttendanceRecord>> GetRecordsBySessionIdAsync(int sessionId)
        {
            return await _context.AttendanceRecords
                .Where(r => r.AttendanceSessionId == sessionId)
                .ToListAsync();
        }

        public async Task<List<AttendanceSession>> GetSessionsByInstructorAsync(int instructorId)
        {
            return await _context.AttendanceSessions
                .Include(s => s.CourseClass)
                    .ThenInclude(c => c.Course)
                .Include(s => s.Records)
                .Where(s => s.InstructorId == instructorId)
                .OrderByDescending(s => s.SessionDate)
                .ToListAsync();
        }

        public async Task<List<AttendanceRecord>> GetRecordsByStudentAsync(int studentId, DateTime fromDate, DateTime toDate)
        {
            return await _context.AttendanceRecords
                .Include(r => r.AttendanceSession)
                    .ThenInclude(s => s.CourseClass)
                        .ThenInclude(c => c.Course)
                .Where(r => r.StudentId == studentId
                            && r.AttendanceSession.SessionDate >= fromDate.Date
                            && r.AttendanceSession.SessionDate <= toDate.Date)
                .OrderByDescending(r => r.AttendanceSession.SessionDate)
                .ToListAsync();
        }

        public async Task AddRecordsAsync(IEnumerable<AttendanceRecord> records)
        {
            await _context.AttendanceRecords.AddRangeAsync(records);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}