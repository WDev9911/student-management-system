using StudentManagementSystem.BLL.DTOs.Attendance;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface IAttendanceService
    {
        Task<AttendanceSessionDto?> GetAttendanceSessionAsync(int classId, int instructorId, DateTime sessionDate);
        Task<bool> SaveAttendanceAsync(AttendanceSaveRequest request, int instructorId);
        Task<List<InstructorAttendanceSessionDto>> GetInstructorSessionsAsync(int instructorId);
        Task<List<StudentAttendanceDto>> GetStudentAttendanceAsync(int studentId, DateTime fromDate, DateTime toDate);
        Task<List<StudentAttendanceClassDto>> GetStudentClassesAsync(int studentId);
    }
}