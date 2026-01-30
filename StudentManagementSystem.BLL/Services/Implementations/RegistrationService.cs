using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IRegistrationRequestRepository _regRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly IUserRepository _userRepo;
        private readonly ApplicationDbContext _db;

        public RegistrationService(
            IRegistrationRequestRepository regRepo,
            IStudentRepository studentRepo,
            IUserRepository userRepo,
            ApplicationDbContext db)
        {
            _regRepo = regRepo;
            _studentRepo = studentRepo;
            _userRepo = userRepo;
            _db = db;
        }

        public async Task<IEnumerable<MajorDto>> GetMajorsAsync()
        {
            return await _db.Majors
                .Select(m => new MajorDto
                {
                    MajorId = m.MajorId,
                    MajorName = m.MajorName
                })
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> RegisterAsync(RegisterRequestDto dto)
        {
            if (await _regRepo.IsUsernameExistsAsync(dto.Username))
            {
                return (false, "Username already exists");
            }

            if (await _regRepo.IsEmailExistsInPendingAsync(dto.Email))
            {
                return (false, "Email already has a pending registration");
            }

            var request = new RegistrationRequest
            {
                FullName = dto.FullName,
                Email = dto.Email,
                Phone = dto.Phone,
                DateOfBirth = dto.DateOfBirth,
                Gender = dto.Gender,
                Address = dto.Address,
                MajorId = dto.MajorId,
                Username = dto.Username,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                Status = "Pending",
                RequestDate = DateTime.Now
            };

            await _regRepo.CreateAsync(request);
            return (true, "Registration request submitted successfully");
        }

        public async Task<IEnumerable<RegistrationRequestListDto>> GetPendingRequestsAsync()
        {
            var requests = await _regRepo.GetPendingRequestsAsync();
            return requests.Select(r => new RegistrationRequestListDto
            {
                RequestId = r.RequestId,
                FullName = r.FullName,
                Email = r.Email,
                Username = r.Username,
                RequestDate = r.RequestDate,
                Status = r.Status
            });
        }

        public async Task<(bool Success, string Message)> ApproveRequestAsync(int requestId, int adminUserId)
        {
            var request = await _regRepo.GetByIdAsync(requestId);
            if (request == null)
            {
                return (false, "Request not found");
            }

            if (request.Status != "Pending")
            {
                return (false, "Request already processed");
            }

            var studentCode = await _studentRepo.GenerateStudentCodeAsync();

            var student = new Student
            {
                StudentCode = studentCode,
                FullName = request.FullName,
                Email = request.Email,
                Phone = request.Phone,
                DateOfBirth = request.DateOfBirth,
                Gender = request.Gender,
                Address = request.Address,
                MajorId = request.MajorId,
                CurrentSemester = 1,
                Status = "Active",
                EnrollmentDate = DateTime.Now
            };

            var createdStudent = await _studentRepo.CreateAsync(student);

            var user = new User
            {
                Username = request.Username,
                PasswordHash = request.PasswordHash,
                Role = "Student",
                StudentId = createdStudent.StudentId,
                IsActive = true,
                CreatedDate = DateTime.Now
            };

            await _userRepo.CreateUserAsync(user);

            var wallet = new Wallet
            {
                StudentId = createdStudent.StudentId,
                Balance = 0,
                CreatedDate = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            await _userRepo.CreateWalletAsync(wallet);

            request.Status = "Approved";
            request.ProcessedDate = DateTime.Now;
            request.ProcessedByUserId = adminUserId;
            request.CreatedStudentId = createdStudent.StudentId;

            await _regRepo.UpdateAsync(request);

            return (true, $"Student {studentCode} created successfully");
        }

        public async Task<(bool Success, string Message)> RejectRequestAsync(int requestId, int adminUserId, string reason)
        {
            var request = await _regRepo.GetByIdAsync(requestId);
            if (request == null)
            {
                return (false, "Request not found");
            }

            if (request.Status != "Pending")
            {
                return (false, "Request already processed");
            }

            request.Status = "Rejected";
            request.ProcessedDate = DateTime.Now;
            request.ProcessedByUserId = adminUserId;
            request.RejectionReason = reason;

            await _regRepo.UpdateAsync(request);

            return (true, "Request rejected");
        }
    }
}