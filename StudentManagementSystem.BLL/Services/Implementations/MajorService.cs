using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class MajorService : IMajorService
    {
        private readonly IMajorRepository _majorRepository;

        public MajorService(IMajorRepository majorRepository)
        {
            _majorRepository = majorRepository;
        }

        public async Task<List<MajorDto>> GetAllMajorsAsync()
        {
            var majors = await _majorRepository.GetAllMajorsAsync();
            
            return majors.Select(m => new MajorDto
            {
                MajorId = m.MajorId,
                MajorCode = m.MajorCode,
                MajorName = m.MajorName,
                Description = m.Description,
                DurationYears = m.DurationYears,
                TotalSemesters = m.TotalSemesters,
                TotalCredits = m.TotalCredits
            }).ToList();
        }

        public async Task<MajorDto?> GetMajorByIdAsync(int id)
        {
            var major = await _majorRepository.GetMajorByIdAsync(id);
            if (major == null)
            {
                return null;
            }

            return new MajorDto
            {
                MajorId = major.MajorId,
                MajorCode = major.MajorCode,
                MajorName = major.MajorName,
                Description = major.Description,
                DurationYears = major.DurationYears,
                TotalSemesters = major.TotalSemesters,
                TotalCredits = major.TotalCredits
            };
        }
    }
}