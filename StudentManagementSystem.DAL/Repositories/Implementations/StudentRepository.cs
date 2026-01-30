using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Data;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.DAL.Repositories.Implementations
{
    public class StudentRepository : IStudentRepository
    {
        private readonly ApplicationDbContext _db;

        public StudentRepository(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<Student> CreateAsync(Student student)
        {
            await _db.Students.AddAsync(student);
            await _db.SaveChangesAsync();
            return student;
        }

        public async Task<string> GenerateStudentCodeAsync()
        {
            var lastStudent = await _db.Students
                .OrderByDescending(s => s.StudentId)
                .FirstOrDefaultAsync();

            if (lastStudent == null)
            {
                return "SV001";
            }

            var lastNumber = int.Parse(lastStudent.StudentCode.Substring(2));
            return $"SV{(lastNumber + 1):D3}";
        }

        public Task<Student?> GetByIdWithDetailsAsync(int studentId)
        {
            return _db.Students
                .Include(s => s.Major)
                .Include(s => s.Wallet)
                .Include(s => s.User)  // ?? ADD THIS LINE
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
        }

        public async Task<decimal> GetWalletBalanceAsync(int studentId)
        {
            var wallet = await _db.Wallets.FirstOrDefaultAsync(w => w.StudentId == studentId);
            return wallet?.Balance ?? 0;
        }

        public Task<int> GetEnrolledCoursesCountAsync(int studentId)
        {
            return _db.Enrollments
                .Where(e => e.StudentId == studentId && e.Status == "Active")
                .CountAsync();
        }

        public Task<List<Enrollment>> GetActiveEnrollmentsAsync(int studentId)
        {
            return _db.Enrollments
                .Include(e => e.Course)
                .Include(e => e.CourseClass)
                .Where(e => e.StudentId == studentId && e.Status == "Active")
                .ToListAsync();
        }
    }
}