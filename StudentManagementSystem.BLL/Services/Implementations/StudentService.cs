using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public partial class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;

        public StudentService(
            IStudentRepository studentRepo,
            IWalletRepository walletRepo,
            ITransactionRepository transactionRepo)
        {
            _studentRepo = studentRepo;
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
        }

        public async Task<StudentDashboardDto?> GetDashboardDataAsync(int studentId)
        {
            var student = await _studentRepo.GetByIdWithDetailsAsync(studentId);
            if (student == null) return null;

            var walletBalance = await _studentRepo.GetWalletBalanceAsync(studentId);
            var enrolledCount = await _studentRepo.GetEnrolledCoursesCountAsync(studentId);
            var activeEnrollments = await _studentRepo.GetActiveEnrollmentsAsync(studentId);

            var completedCredits = activeEnrollments
                .Where(e => e.Grade != null && e.Grade.IsPassed)
                .Sum(e => e.Course.Credits);

            var weekSchedule = activeEnrollments
                .Where(e => e.CourseClass != null)
                .GroupBy(e => e.CourseClass.DayOfWeek)
                .Select(g => new WeekScheduleDto
                {
                    DayOfWeek = g.Key,
                    Classes = g.Select(e => new ClassSessionDto
                    {
                        CourseName = e.Course.CourseName,
                        StartTime = e.CourseClass.StartTime,
                        EndTime = e.CourseClass.EndTime,
                        RoomNumber = e.CourseClass.Room
                    }).ToList()
                })
                .ToList();

            return new StudentDashboardDto
            {
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                WalletBalance = walletBalance,
                EnrolledCoursesCount = enrolledCount,
                CurrentGPA = student.CGPA,
                CreditsCompleted = completedCredits,
                CurrentSemester = student.CurrentSemester,
                TotalCreditsRequired = student.Major?.TotalCredits ?? 120,
                RecentNotifications = new List<RecentNotificationDto>(),
                ThisWeekSchedule = weekSchedule
            };
        }

        public async Task<WalletInfoDto?> GetWalletInfoAsync(int studentId)
        {
            // Use existing repository method that returns student with details
            var student = await _studentRepo.GetByIdWithDetailsAsync(studentId);
            if (student == null) return null;

            var wallet = await _walletRepo.GetByStudentIdAsync(studentId);
            var transactions = await _transactionRepo.GetRecentTransactionsAsync(studentId, 20);

            return new WalletInfoDto
            {
                WalletId = wallet?.WalletId ?? 0,
                StudentId = studentId,
                StudentCode = student.StudentCode,
                StudentName = student.FullName,
                Balance = wallet?.Balance ?? 0,
                LastUpdated = wallet?.LastUpdated ?? DateTime.Now,
                RecentTransactions = transactions.Select(t => new TransactionDto
                {
                    TransactionId = t.TransactionId,
                    TransactionCode = t.TransactionCode,
                    Type = t.Type,
                    Amount = t.Amount,
                    Description = t.Description ?? string.Empty,
                    Status = t.Status,
                    PaymentMethod = t.PaymentMethod,
                    CreatedDate = t.CreatedDate
                }).ToList()
            };
        }

        // ... other StudentService methods
    }
}