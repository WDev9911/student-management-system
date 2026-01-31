using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class SemesterService : ISemesterService
    {
        private readonly ISemesterRepository _semesterRepository;

        public SemesterService(ISemesterRepository semesterRepository)
        {
            _semesterRepository = semesterRepository;
        }

        public async Task<List<SemesterDto>> GetAllSemestersAsync()
        {
            var semesters = await _semesterRepository.GetAllSemestersAsync();
            
            return semesters.Select(s => new SemesterDto
            {
                SemesterId = s.SemesterId,
                SemesterNumber = s.SemesterNumber,
                SemesterName = s.SemesterName,
                AcademicYear = s.AcademicYear ?? string.Empty,
                StartDate = s.StartDate,
                EndDate = s.EndDate,
                Status = s.Status ?? "Upcoming"
            }).ToList();
        }

        public async Task<SemesterDto?> GetSemesterByIdAsync(int id)
        {
            var semester = await _semesterRepository.GetSemesterByIdAsync(id);
            if (semester == null)
            {
                return null;
            }

            return new SemesterDto
            {
                SemesterId = semester.SemesterId,
                SemesterNumber = semester.SemesterNumber,
                SemesterName = semester.SemesterName,
                AcademicYear = semester.AcademicYear ?? string.Empty,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate,
                Status = semester.Status ?? "Upcoming"
            };
        }

        public async Task<SemesterDto?> GetCurrentSemesterAsync()
        {
            var semester = await _semesterRepository.GetCurrentSemesterAsync();
            if (semester == null)
            {
                return null;
            }

            return new SemesterDto
            {
                SemesterId = semester.SemesterId,
                SemesterNumber = semester.SemesterNumber,
                SemesterName = semester.SemesterName,
                AcademicYear = semester.AcademicYear ?? string.Empty,
                StartDate = semester.StartDate,
                EndDate = semester.EndDate,
                Status = semester.Status ?? "Active"
            };
        }

        public async Task<(bool Success, string Message)> UpdateSemesterDatesAsync(int semesterId, DateTime startDate, DateTime endDate)
        {
            if (endDate < startDate)
            {
                return (false, "Ngày k?t thúc ph?i l?n h?n ho?c b?ng ngày b?t ??u.");
            }

            var semester = await _semesterRepository.GetSemesterByIdAsync(semesterId);
            if (semester == null)
            {
                return (false, "Không tìm th?y h?c k?.");
            }

            semester.StartDate = startDate;
            semester.EndDate = endDate;

            await _semesterRepository.UpdateSemesterAsync(semester);
            return (true, "C?p nh?t ngày h?c k? thành công.");
        }
    }
}