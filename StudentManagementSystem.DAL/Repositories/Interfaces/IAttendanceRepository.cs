using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Repositories.Interfaces
{
    public interface IAttendanceRepository
    {
        Task<AttendanceSession?> GetSessionWithRecordsAsync(int classId, DateTime sessionDate);
        Task<AttendanceSession?> GetSessionAsync(int classId, DateTime sessionDate);
        Task CreateSessionAsync(AttendanceSession session);
        Task<List<AttendanceRecord>> GetRecordsBySessionIdAsync(int sessionId);
        Task<List<AttendanceSession>> GetSessionsByInstructorAsync(int instructorId);
        Task<List<AttendanceRecord>> GetRecordsByStudentAsync(int studentId, DateTime fromDate, DateTime toDate);
        Task AddRecordsAsync(IEnumerable<AttendanceRecord> records);
        Task SaveChangesAsync();
    }
}