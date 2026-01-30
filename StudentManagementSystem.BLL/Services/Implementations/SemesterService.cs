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
    }
}