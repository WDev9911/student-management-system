using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class InstructorService : IInstructorService
    {
        private readonly IInstructorRepository _instructorRepo;
        private readonly IUserRepository _userRepo;

        public InstructorService(IInstructorRepository instructorRepo, IUserRepository userRepo)
        {
            _instructorRepo = instructorRepo;
            _userRepo = userRepo;
        }

        public async Task<(bool Success, string Message)> CreateInstructorAsync(CreateInstructorDto dto)
        {
            // Check email exists using correct method
            if (await _instructorRepo.EmailExistsAsync(dto.Email))
            {
                return (false, "Email ?ã t?n t?i");
            }

            // Generate code
            var allInstructors = await _instructorRepo.GetAllInstructorsAsync();
            var maxCode = allInstructors
                .Select(i => int.TryParse(i.InstructorCode.Replace("GV", ""), out var num) ? num : 0)
                .DefaultIfEmpty(0)
                .Max();
            var instructorCode = $"GV{(maxCode + 1):D3}";

            var instructor = new Instructor
            {
                InstructorCode = instructorCode,
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                Department = dto.Department,
                Specialization = dto.Specialization,
                Qualifications = dto.Qualifications,
                Status = "Active",
                CreatedDate = DateTime.Now
            };

            var createdInstructor = await _instructorRepo.AddInstructorAsync(instructor);

            // Create user
            var user = new User
            {
                Username = instructorCode.ToLower(),
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("123456"),
                Role = "Instructor",
                InstructorId = createdInstructor.InstructorId,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _userRepo.CreateUserAsync(user);

            return (true, $"T?o GV {instructorCode} thành công. User: {instructorCode.ToLower()}, Pass: 123456");
        }

        public async Task<IEnumerable<InstructorListDto>> GetAllInstructorsAsync()
        {
            var instructors = await _instructorRepo.GetAllInstructorsAsync();
            return instructors.Select(i => new InstructorListDto
            {
                InstructorId = i.InstructorId,
                InstructorCode = i.InstructorCode,
                FullName = i.FullName,
                Email = i.Email,
                Phone = i.Phone,
                Department = i.Department,
                Specialization = i.Specialization,
                Status = i.Status
            });
        }

        public async Task<InstructorDetailDto?> GetInstructorByIdAsync(int instructorId)
        {
            var instructor = await _instructorRepo.GetInstructorByIdAsync(instructorId);
            if (instructor == null) return null;

            return new InstructorDetailDto
            {
                InstructorId = instructor.InstructorId,
                InstructorCode = instructor.InstructorCode,
                FullName = instructor.FullName,
                Email = instructor.Email,
                Phone = instructor.Phone,
                Department = instructor.Department,
                Specialization = instructor.Specialization,
                Qualifications = instructor.Qualifications,
                Status = instructor.Status,
                CreatedDate = instructor.CreatedDate
            };
        }

        public async Task<(bool Success, string Message)> UpdateInstructorAsync(int instructorId, CreateInstructorDto dto)
        {
            var instructor = await _instructorRepo.GetInstructorByIdAsync(instructorId);
            if (instructor == null) return (false, "Không tìm th?y GV");

            instructor.FullName = dto.FullName;
            instructor.Email = dto.Email;
            instructor.Phone = dto.Phone;
            instructor.Department = dto.Department;
            instructor.Specialization = dto.Specialization;
            instructor.Qualifications = dto.Qualifications;

            await _instructorRepo.UpdateInstructorAsync(instructor);
            return (true, "C?p nh?t thành công");
        }

        public async Task<(bool Success, string Message)> DeleteInstructorAsync(int instructorId)
        {
            var instructor = await _instructorRepo.GetInstructorByIdAsync(instructorId);
            if (instructor == null) return (false, "Không tìm th?y GV");

            await _instructorRepo.DeleteInstructorAsync(instructorId);
            return (true, "Xóa thành công");
        }
    }
}