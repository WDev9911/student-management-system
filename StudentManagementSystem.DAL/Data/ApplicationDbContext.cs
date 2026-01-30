using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.DAL.Models;

namespace StudentManagementSystem.DAL.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets cho 15 tables
        public DbSet<Major> Majors { get; set; }
        public DbSet<Semester> Semesters { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<CourseClass> CourseClasses { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Grade> Grades { get; set; }
        public DbSet<InstructorAssignment> InstructorAssignments { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Invoice> Invoices { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<RegistrationRequest> RegistrationRequests { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ========================================
            // CONFIGURE RELATIONSHIPS & CONSTRAINTS
            // ========================================

            // 1. Student - Unique constraints
            modelBuilder.Entity<Student>()
                .HasIndex(s => s.StudentCode)
                .IsUnique();

            modelBuilder.Entity<Student>()
                .HasIndex(s => s.Email)
                .IsUnique();

            // 2. Instructor - Unique constraints
            modelBuilder.Entity<Instructor>()
                .HasIndex(i => i.InstructorCode)
                .IsUnique();

            modelBuilder.Entity<Instructor>()
                .HasIndex(i => i.Email)
                .IsUnique();

            // 3. User - Unique constraints
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Username)
                .IsUnique();

            // 4. Course - Self-referencing relationship (Prerequisite)
            modelBuilder.Entity<Course>()
                .HasOne(c => c.PrerequisiteCourse)
                .WithMany()
                .HasForeignKey(c => c.PrerequisiteCourseId)
                .OnDelete(DeleteBehavior.Restrict); // Không xóa cascade

            // 5. Student - Wallet (1-1 relationship)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.Wallet)
                .WithOne(w => w.Student)
                .HasForeignKey<Wallet>(w => w.StudentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 6. Student - User (1-1 relationship)
            modelBuilder.Entity<Student>()
                .HasOne(s => s.User)
                .WithOne(u => u.Student)
                .HasForeignKey<User>(u => u.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 7. Instructor - User (1-1 relationship)
            modelBuilder.Entity<Instructor>()
                .HasOne(i => i.User)
                .WithOne(u => u.Instructor)
                .HasForeignKey<User>(u => u.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            // 8. Enrollment - Grade (1-1 relationship)
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Grade)
                .WithOne(g => g.Enrollment)
                .HasForeignKey<Grade>(g => g.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);

            // 9. Major - Unique constraint
            modelBuilder.Entity<Major>()
                .HasIndex(m => m.MajorCode)
                .IsUnique();

            // 10. Course - Unique constraint
            modelBuilder.Entity<Course>()
                .HasIndex(c => c.CourseCode)
                .IsUnique();

            // 11. CourseClass - Unique constraint
            modelBuilder.Entity<CourseClass>()
                .HasIndex(cc => cc.ClassName)
                .IsUnique();

            // 12. Enrollment relationships - Fix cascade paths
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId)
                .OnDelete(DeleteBehavior.Restrict); // ⬅️ QUAN TRỌNG!

            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.CourseClass)
                .WithMany(cc => cc.Enrollments)
                .HasForeignKey(e => e.ClassId)
                .OnDelete(DeleteBehavior.Restrict); // ⬅️ QUAN TRỌNG!

            // 13. Grade relationships - Fix cascade paths
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Course)
                .WithMany(c => c.Grades)
                .HasForeignKey(g => g.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // 14. Enrollment → Student - Fix cascade paths ⬅️ THÊM ĐOẠN NÀY
            modelBuilder.Entity<Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 15. Grade → Student - Fix cascade paths ⬅️ VÀ THÊM ĐOẠN NÀY
            modelBuilder.Entity<Grade>()
                .HasOne(g => g.Student)
                .WithMany(s => s.Grades)
                .HasForeignKey(g => g.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // 16. Transaction relationships - Fix cascade paths ⬅️ THÊM ĐOẠN NÀY
            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Student)
                .WithMany(s => s.Transactions)
                .HasForeignKey(t => t.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(t => t.Wallet)
                .WithMany(w => w.Transactions)
                .HasForeignKey(t => t.WalletId)
                .OnDelete(DeleteBehavior.Restrict);



            // ========================================
            // SEED DATA
            // ========================================
            SeedData(modelBuilder);
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            // ========================================
            // 1. SEED MAJORS (3 chuyên ngành)
            // ========================================
            modelBuilder.Entity<Major>().HasData(
                new Major
                {
                    MajorId = 1,
                    MajorCode = "IT",
                    MajorName = "Công nghệ Thông tin",
                    Description = "Đào tạo kỹ sư phần mềm, lập trình viên",
                    DurationYears = 3,
                    TotalSemesters = 9,
                    TotalCredits = 108,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Major
                {
                    MajorId = 2,
                    MajorCode = "ENG",
                    MajorName = "Ngôn ngữ Anh",
                    Description = "Đào tạo giáo viên tiếng Anh, biên phiên dịch",
                    DurationYears = 3,
                    TotalSemesters = 9,
                    TotalCredits = 108,
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Major
                {
                    MajorId = 3,
                    MajorCode = "BUS",
                    MajorName = "Quản trị Kinh doanh",
                    Description = "Đào tạo nhà quản trị doanh nghiệp",
                    DurationYears = 3,
                    TotalSemesters = 9,
                    TotalCredits = 108,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // ========================================
            // 2. SEED SEMESTERS (9 kỳ)
            // ========================================
            modelBuilder.Entity<Semester>().HasData(
                new Semester { SemesterId = 1, SemesterNumber = 1, SemesterName = "Kỳ 1", AcademicYear = "2024-2025", StartDate = new DateTime(2024, 9, 1), EndDate = new DateTime(2024, 11, 30), Status = "Completed", CreatedDate = new DateTime(2024, 1, 1) },
                new Semester { SemesterId = 2, SemesterNumber = 2, SemesterName = "Kỳ 2", AcademicYear = "2024-2025", StartDate = new DateTime(2024, 12, 1), EndDate = new DateTime(2025, 2, 28), Status = "Active", CreatedDate = new DateTime(2024, 1, 1) },
                new Semester { SemesterId = 3, SemesterNumber = 3, SemesterName = "Kỳ 3", AcademicYear = "2024-2025", StartDate = new DateTime(2025, 3, 1), EndDate = new DateTime(2025, 5, 31), Status = "Upcoming", CreatedDate = new DateTime(2024, 1, 1) },
                new Semester { SemesterId = 4, SemesterNumber = 4, SemesterName = "Kỳ 4", AcademicYear = "2025-2026", StartDate = new DateTime(2025, 9, 1), EndDate = new DateTime(2025, 11, 30), Status = "Upcoming", CreatedDate = new DateTime(2024, 1, 1) },
                new Semester { SemesterId = 5, SemesterNumber = 5, SemesterName = "Kỳ 5", AcademicYear = "2025-2026", StartDate = new DateTime(2025, 12, 1), EndDate = new DateTime(2026, 2, 28), Status = "Upcoming", CreatedDate = new DateTime(2024, 1, 1) },
                new Semester { SemesterId = 6, SemesterNumber = 6, SemesterName = "Kỳ 6", AcademicYear = "2025-2026", StartDate = new DateTime(2026, 3, 1), EndDate = new DateTime(2026, 5, 31), Status = "Upcoming", CreatedDate = new DateTime(2024, 1, 1) },
                new Semester { SemesterId = 7, SemesterNumber = 7, SemesterName = "Kỳ 7", AcademicYear = "2026-2027", StartDate = new DateTime(2026, 9, 1), EndDate = new DateTime(2026, 11, 30), Status = "Upcoming", CreatedDate = new DateTime(2024, 1, 1) },
                new Semester { SemesterId = 8, SemesterNumber = 8, SemesterName = "Kỳ 8", AcademicYear = "2026-2027", StartDate = new DateTime(2026, 12, 1), EndDate = new DateTime(2027, 2, 28), Status = "Upcoming", CreatedDate = new DateTime(2024, 1, 1) },
                new Semester { SemesterId = 9, SemesterNumber = 9, SemesterName = "Kỳ 9", AcademicYear = "2026-2027", StartDate = new DateTime(2027, 3, 1), EndDate = new DateTime(2027, 5, 31), Status = "Upcoming", CreatedDate = new DateTime(2024, 1, 1) }
            );

            // ========================================
            // 3. SEED ADMIN USER
            // ========================================
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    UserId = 1,
                    Username = "admin",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"),
                    Role = "Admin",
                    IsActive = true,
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // ========================================
            // 4. SEED INSTRUCTORS (3 giảng viên mẫu)
            // ========================================
            modelBuilder.Entity<Instructor>().HasData(
                new Instructor
                {
                    InstructorId = 1,
                    InstructorCode = "GV001",
                    FullName = "Nguyễn Văn An",
                    Email = "nva@university.edu.vn",
                    Phone = "0901234567",
                    Specialization = "Database, Programming",
                    Qualifications = "Thạc sĩ CNTT",
                    Status = "Active",
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Instructor
                {
                    InstructorId = 2,
                    InstructorCode = "GV002",
                    FullName = "Trần Thị Bình",
                    Email = "ttb@university.edu.vn",
                    Phone = "0901234568",
                    Specialization = "Web Development, OOP",
                    Qualifications = "Tiến sĩ CNTT",
                    Status = "Active",
                    CreatedDate = new DateTime(2024, 1, 1)
                },
                new Instructor
                {
                    InstructorId = 3,
                    InstructorCode = "GV003",
                    FullName = "Lê Văn Cường",
                    Email = "lvc@university.edu.vn",
                    Phone = "0901234569",
                    Specialization = "Data Structures, Algorithms",
                    Qualifications = "Thạc sĩ CNTT",
                    Status = "Active",
                    CreatedDate = new DateTime(2024, 1, 1)
                }
            );

            // ========================================
            // 5. SEED COURSES - KỲ 1 (12 môn cho 3 chuyên ngành)
            // ========================================

            // IT - Kỳ 1
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 1, CourseCode = "IT101", CourseName = "Nhập môn Lập trình", MajorId = 1, SemesterNumber = 1, Credits = 3, TuitionFee = 2000000, Description = "Học C# cơ bản", CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 2, CourseCode = "IT102", CourseName = "Toán rời rạc", MajorId = 1, SemesterNumber = 1, Credits = 3, TuitionFee = 1500000, Description = "Logic, tập hợp, đồ thị", CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 3, CourseCode = "IT103", CourseName = "Cơ sở dữ liệu", MajorId = 1, SemesterNumber = 1, Credits = 3, TuitionFee = 2000000, Description = "SQL Server cơ bản", CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 4, CourseCode = "IT104", CourseName = "Anh văn IT 1", MajorId = 1, SemesterNumber = 1, Credits = 2, TuitionFee = 1000000, Description = "English for IT", CreatedDate = new DateTime(2024, 1, 1) }
            );

            // English - Kỳ 1
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 5, CourseCode = "ENG101", CourseName = "Grammar Foundation", MajorId = 2, SemesterNumber = 1, Credits = 3, TuitionFee = 1500000, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 6, CourseCode = "ENG102", CourseName = "Listening Skills", MajorId = 2, SemesterNumber = 1, Credits = 3, TuitionFee = 1500000, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 7, CourseCode = "ENG103", CourseName = "Speaking Skills", MajorId = 2, SemesterNumber = 1, Credits = 3, TuitionFee = 1500000, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 8, CourseCode = "ENG104", CourseName = "Phonetics", MajorId = 2, SemesterNumber = 1, Credits = 2, TuitionFee = 1000000, CreatedDate = new DateTime(2024, 1, 1) }
            );

            // Business - Kỳ 1
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 9, CourseCode = "BUS101", CourseName = "Nguyên lý Kinh tế", MajorId = 3, SemesterNumber = 1, Credits = 3, TuitionFee = 1500000, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 10, CourseCode = "BUS102", CourseName = "Quản trị học", MajorId = 3, SemesterNumber = 1, Credits = 3, TuitionFee = 1500000, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 11, CourseCode = "BUS103", CourseName = "Kế toán cơ bản", MajorId = 3, SemesterNumber = 1, Credits = 3, TuitionFee = 1500000, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 12, CourseCode = "BUS104", CourseName = "Anh văn Thương mại", MajorId = 3, SemesterNumber = 1, Credits = 2, TuitionFee = 1000000, CreatedDate = new DateTime(2024, 1, 1) }
            );

            // ========================================
            // 6. SEED COURSES - KỲ 2 (có Prerequisite)
            // ========================================

            // IT - Kỳ 2
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 13, CourseCode = "IT201", CourseName = "Cấu trúc dữ liệu", MajorId = 1, SemesterNumber = 2, Credits = 4, TuitionFee = 2500000, PrerequisiteCourseId = 1, Description = "Stack, Queue, Tree", CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 14, CourseCode = "IT202", CourseName = "Lập trình OOP", MajorId = 1, SemesterNumber = 2, Credits = 4, TuitionFee = 2500000, PrerequisiteCourseId = 1, Description = "C# OOP nâng cao", CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 15, CourseCode = "IT203", CourseName = "SQL Server Nâng cao", MajorId = 1, SemesterNumber = 2, Credits = 3, TuitionFee = 2000000, PrerequisiteCourseId = 3, Description = "Stored Procedure, Trigger", CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 16, CourseCode = "IT204", CourseName = "Anh văn IT 2", MajorId = 1, SemesterNumber = 2, Credits = 2, TuitionFee = 1000000, PrerequisiteCourseId = 4, CreatedDate = new DateTime(2024, 1, 1) }
            );

            // English - Kỳ 2
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 17, CourseCode = "ENG201", CourseName = "Advanced Grammar", MajorId = 2, SemesterNumber = 2, Credits = 3, TuitionFee = 1500000, PrerequisiteCourseId = 5, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 18, CourseCode = "ENG202", CourseName = "Reading Comprehension", MajorId = 2, SemesterNumber = 2, Credits = 3, TuitionFee = 1500000, PrerequisiteCourseId = 6, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 19, CourseCode = "ENG203", CourseName = "Writing Skills", MajorId = 2, SemesterNumber = 2, Credits = 3, TuitionFee = 1500000, PrerequisiteCourseId = 7, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 20, CourseCode = "ENG204", CourseName = "English Literature", MajorId = 2, SemesterNumber = 2, Credits = 2, TuitionFee = 1000000, CreatedDate = new DateTime(2024, 1, 1) }
            );

            // Business - Kỳ 2
            modelBuilder.Entity<Course>().HasData(
                new Course { CourseId = 21, CourseCode = "BUS201", CourseName = "Marketing căn bản", MajorId = 3, SemesterNumber = 2, Credits = 3, TuitionFee = 1500000, PrerequisiteCourseId = 10, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 22, CourseCode = "BUS202", CourseName = "Quản trị Nhân sự", MajorId = 3, SemesterNumber = 2, Credits = 3, TuitionFee = 1500000, PrerequisiteCourseId = 10, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 23, CourseCode = "BUS203", CourseName = "Tài chính Doanh nghiệp", MajorId = 3, SemesterNumber = 2, Credits = 3, TuitionFee = 1500000, PrerequisiteCourseId = 11, CreatedDate = new DateTime(2024, 1, 1) },
                new Course { CourseId = 24, CourseCode = "BUS204", CourseName = "Thống kê Kinh doanh", MajorId = 3, SemesterNumber = 2, Credits = 2, TuitionFee = 1000000, CreatedDate = new DateTime(2024, 1, 1) }
            );

            // Note: Kỳ 3-9 có thể seed thêm sau hoặc để trống
        }
    }
}