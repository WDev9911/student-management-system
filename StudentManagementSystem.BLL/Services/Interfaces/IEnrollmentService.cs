using StudentManagementSystem.BLL.DTOs.CourseRegistration;

namespace StudentManagementSystem.BLL.Services.Interfaces
{
    public interface IEnrollmentService
    {
        Task<List<SemesterCoursesDto>> GetCourseRegistrationDataAsync(int studentId);
        Task<List<PendingCourseDto>> GetPendingCoursesAsync(int studentId);
        Task<RetakeRegistrationDto> GetRetakeRegistrationDataAsync(int studentId);
        Task<EnrollmentResultDto> EnrollStudentAsync(EnrollmentRequestDto request);
        Task<bool> CheckScheduleConflictAsync(int studentId, List<int> classIds);
        Task<decimal> CalculateTotalFeeAsync(List<int> classIds);
        Task<decimal> CalculateTotalFeeAsync(int studentId, List<int> classIds);
    }
}