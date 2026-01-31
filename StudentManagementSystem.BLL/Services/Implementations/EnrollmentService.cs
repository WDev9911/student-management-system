using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.BLL.DTOs.CourseRegistration;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly IClassRepository _classRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly ITransactionRepository _transactionRepo;
        private readonly IGradeRepository _gradeRepo;
        private readonly IPendingEnrollmentRepository _pendingRepo;

        public EnrollmentService(
            IStudentRepository studentRepo,
            ICourseRepository courseRepo,
            IClassRepository classRepo,
            IEnrollmentRepository enrollmentRepo,
            IWalletRepository walletRepo,
            ITransactionRepository transactionRepo,
            IGradeRepository gradeRepo,
            IPendingEnrollmentRepository pendingRepo)
        {
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
            _classRepo = classRepo;
            _enrollmentRepo = enrollmentRepo;
            _walletRepo = walletRepo;
            _transactionRepo = transactionRepo;
            _gradeRepo = gradeRepo;
            _pendingRepo = pendingRepo;
        }

        public async Task<List<SemesterCoursesDto>> GetCourseRegistrationDataAsync(int studentId)
        {
            var student = await _studentRepo.GetByIdWithDetailsAsync(studentId);
            if (student == null) throw new Exception("Student not found");

            var result = new List<SemesterCoursesDto>();

            var allCourses = await _courseRepo.GetCoursesByMajorAsync(student.MajorId);
            var activeEnrollments = await _enrollmentRepo.GetActiveEnrollmentsByStudentIdAsync(studentId);

            var publishedGrades = await _gradeRepo.GetPublishedGradesByStudentIdAsync(studentId);

            var studentGrades = publishedGrades
                .Where(g => g.IsPassed)
                .Select(g => g.CourseId)
                .Distinct()
                .ToList();

            var currentSemesterCourseIds = activeEnrollments
                .Where(e => e.SemesterNumber == student.CurrentSemester)
                .Select(e => e.CourseId)
                .Distinct()
                .ToList();

            var publishedCurrentCount = publishedGrades
                .Where(g => g.Enrollment.SemesterNumber == student.CurrentSemester)
                .Select(g => g.CourseId)
                .Distinct()
                .Count();

            var allowNextSemester = currentSemesterCourseIds.Count > 0 &&
                                    publishedCurrentCount > 0;

            var openSemester = allowNextSemester
                ? Math.Min(student.CurrentSemester + 1, 9)
                : student.CurrentSemester;

            var failedCourseIds = publishedGrades
                .Where(g => !g.IsPassed)
                .Select(g => g.CourseId)
                .Distinct()
                .ToList();

            foreach (var courseId in failedCourseIds)
            {
                var course = allCourses.FirstOrDefault(c => c.CourseId == courseId);
                if (course == null) continue;

                var existing = await _pendingRepo.GetPendingAsync(studentId, courseId, openSemester);
                if (existing == null)
                {
                    await _pendingRepo.AddAsync(new PendingEnrollment
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                        SemesterNumber = openSemester,
                        IsRetake = true,
                        RequiredFee = Math.Round(course.TuitionFee * 0.5m, 0),
                        Reason = "Failed course",
                        Status = "Pending",
                        CreatedDate = DateTime.Now
                    });
                }
            }

            await _pendingRepo.SaveChangesAsync();

            var pendingList = await _pendingRepo.GetPendingByStudentIdAsync(studentId);

            for (int semesterNum = 1; semesterNum <= 9; semesterNum++)
            {
                var semesterCourses = allCourses.Where(c => c.SemesterNumber == semesterNum).ToList();
                var semesterEnrollments = activeEnrollments
                    .Where(e => e.SemesterNumber == semesterNum)
                    .ToList();

                var registeredByCourseId = semesterEnrollments
                    .GroupBy(e => e.CourseId)
                    .ToDictionary(
                        g => g.Key,
                        g => g.OrderByDescending(e => e.EnrollmentDate).First()
                    );

                var hasRegistered = semesterEnrollments.Any();

                var semesterDto = new SemesterCoursesDto
                {
                    SemesterNumber = semesterNum,
                    SemesterName = $"Semester {semesterNum}",
                    IsCurrentSemester = semesterNum == student.CurrentSemester,
                    CanRegister = semesterNum == student.CurrentSemester || (allowNextSemester && semesterNum == openSemester),
                    IsRegistrationLocked = false,
                    HasRegisteredCourses = hasRegistered,
                    RegisteredTotalFee = hasRegistered ? semesterEnrollments.Sum(e => e.PaidAmount) : 0,
                    RegisteredCourses = hasRegistered
                        ? semesterEnrollments.Select(e => new RegisteredCourseDto
                        {
                            ClassId = e.ClassId,
                            CourseCode = e.Course.CourseCode,
                            CourseName = e.Course.CourseName,
                            ClassName = e.CourseClass.ClassName,
                            Schedule = e.CourseClass.Schedule,
                            Room = e.CourseClass.Room,
                            InstructorName = e.CourseClass.Instructor?.FullName ?? "TBA",
                            TuitionFee = e.PaidAmount
                        }).ToList()
                        : new List<RegisteredCourseDto>(),
                    Courses = new List<CourseWithClassesDto>()
                };

                foreach (var course in semesterCourses)
                {
                    bool isEligible = true;
                    string? blockReason = null;
                    var isBlockedByPrereq = false;

                    if (course.PrerequisiteCourseId.HasValue &&
                        !studentGrades.Contains(course.PrerequisiteCourseId.Value))
                    {
                        isEligible = false;
                        isBlockedByPrereq = true;
                        var prereqCourse = allCourses.FirstOrDefault(c => c.CourseId == course.PrerequisiteCourseId.Value);
                        blockReason = $"Prerequisite required: {prereqCourse?.CourseName}";
                    }

                    registeredByCourseId.TryGetValue(course.CourseId, out var registered);

                    var courseDto = new CourseWithClassesDto
                    {
                        CourseId = course.CourseId,
                        CourseCode = course.CourseCode,
                        CourseName = course.CourseName,
                        Credits = course.Credits,
                        TuitionFee = course.TuitionFee,
                        IsEligible = isEligible,
                        BlockReason = blockReason,
                        PrerequisiteCourseId = course.PrerequisiteCourseId,
                        IsAlreadyRegistered = registered != null,
                        RegisteredClassId = registered?.ClassId,
                        RegisteredClassDisplay = registered == null
                            ? null
                            : $"{registered.CourseClass.ClassName} - {registered.CourseClass.Schedule} - {registered.CourseClass.Room} ({registered.CourseClass.Instructor?.FullName ?? "TBA"})",
                        AvailableClasses = new List<ClassOptionDto>()
                    };

                    // N?u k? m?i b? v??ng prerequisite -> Pending list (100% h?c phí)
                    if (semesterNum == openSemester && isBlockedByPrereq)
                    {
                        var prereqName = allCourses
    .FirstOrDefault(c => c.CourseId == course.PrerequisiteCourseId)?.CourseName
    ?? "the prerequisite course";

                        var existing = await _pendingRepo.GetPendingAsync(studentId, course.CourseId, openSemester);
                        if (existing == null)
                        {
                            await _pendingRepo.AddAsync(new PendingEnrollment
                            {
                                StudentId = studentId,
                                CourseId = course.CourseId,
                                SemesterNumber = openSemester,
                                IsRetake = false,
                                RequiredFee = Math.Round(course.TuitionFee, 0),
                                Reason = $"You must pass prerequisite {prereqName} before you can register this course.",
                                Status = "Pending",
                                CreatedDate = DateTime.Now
                            });
                        }
                    }

                    if (semesterDto.CanRegister && isEligible)
                    {
                        var classes = await _classRepo.GetClassesByCourseAsync(course.CourseId);
                        courseDto.AvailableClasses = classes.Select(c => new ClassOptionDto
                        {
                            ClassId = c.ClassId,
                            ClassName = c.ClassName,
                            Schedule = c.Schedule,
                            DayOfWeek = c.DayOfWeek,
                            StartTime = c.StartTime,
                            EndTime = c.EndTime,
                            Room = c.Room,
                            InstructorName = c.Instructor?.FullName ?? "TBA",
                            CurrentEnrollment = c.CurrentEnrollment,
                            MaxStudents = c.MaxStudents,
                            StartDate = c.StartDate,
                            EndDate = c.EndDate
                        }).ToList();
                    }

                    semesterDto.Courses.Add(courseDto);
                }

                semesterDto.PendingCourses = pendingList
                    .Where(p => p.SemesterNumber == semesterNum)
                    .Select(p => new PendingCourseDto
                    {
                        CourseId = p.CourseId,
                        CourseCode = p.Course.CourseCode,
                        CourseName = p.Course.CourseName,
                        RequiredFee = p.RequiredFee,
                        Reason = p.Reason
                    }).ToList();

                result.Add(semesterDto);
            }

            return result;
        }

        public async Task<EnrollmentResultDto> EnrollStudentAsync(EnrollmentRequestDto request)
        {
            var result = new EnrollmentResultDto { Success = false };

            try
            {
                var student = await _studentRepo.GetByIdWithDetailsAsync(request.StudentId);
                if (student == null)
                {
                    result.Message = "Student not found";
                    return result;
                }

                var wallet = await _walletRepo.GetByStudentIdAsync(request.StudentId);
                if (wallet == null)
                {
                    result.Message = "Wallet not found";
                    return result;
                }

                if (await CheckScheduleConflictAsync(request.StudentId, request.SelectedClassIds))
                {
                    result.Message = "Schedule conflict detected";
                    result.Errors.Add("Selected classes have overlapping schedules");
                    return result;
                }

                var totalFee = await CalculateTotalFeeAsync(request.StudentId, request.SelectedClassIds);

                var pending = await _pendingRepo.GetPendingByStudentIdAsync(request.StudentId);
                var pendingCourseIds = pending.Where(p => p.IsRetake).Select(p => p.CourseId).ToHashSet();

                foreach (var classId in request.SelectedClassIds)
                {
                    var classInfo = await _classRepo.GetByIdAsync(classId);
                    if (classInfo == null) continue;

                    var isRetake = pendingCourseIds.Contains(classInfo.CourseId);
                    var paidAmount = isRetake
                        ? Math.Round(classInfo.Course.TuitionFee * 0.5m, 0)
                        : classInfo.Course.TuitionFee;

                    var enrollment = new Enrollment
                    {
                        StudentId = request.StudentId,
                        ClassId = classId,
                        CourseId = classInfo.CourseId,
                        SemesterNumber = student.CurrentSemester,
                        EnrollmentDate = DateTime.Now,
                        Status = "Active",
                        PaidAmount = paidAmount,
                        IsRetake = isRetake
                    };

                    await _enrollmentRepo.CreateEnrollmentAsync(enrollment);

                    classInfo.CurrentEnrollment++;
                    await _classRepo.SaveChangesAsync();

                    result.EnrolledCourses.Add($"{classInfo.Course.CourseCode} - {classInfo.ClassName}");
                }

                wallet.Balance -= totalFee;
                wallet.LastUpdated = DateTime.Now;
                await _walletRepo.SaveChangesAsync();

                var transaction = new Transaction
                {
                    WalletId = wallet.WalletId,
                    StudentId = request.StudentId,
                    TransactionCode = $"ENROLL-{DateTime.Now:yyyyMMddHHmmss}",
                    Type = "Payment",
                    Amount = totalFee,
                    BalanceBefore = wallet.Balance + totalFee,
                    BalanceAfter = wallet.Balance,
                    Description = $"Course registration for {request.SelectedClassIds.Count} courses in semester {student.CurrentSemester}",
                    Status = "Completed",
                    PaymentMethod = "Wallet",
                    CreatedDate = DateTime.Now
                };

                await _transactionRepo.CreateAsync(transaction);

                result.Success = true;
                result.Message = "Registration successful!";
                result.TotalFee = totalFee;
                result.RemainingBalance = wallet.Balance;

                return result;
            }
            catch (Exception ex)
            {
                result.Message = "An error occurred";
                result.Errors.Add(ex.Message);
                return result;
            }
        }

        public async Task<bool> CheckScheduleConflictAsync(int studentId, List<int> classIds)
        {
            foreach (var classId in classIds)
            {
                if (await _enrollmentRepo.HasScheduleConflictAsync(studentId, classId))
                {
                    return true;
                }
            }

            var classes = new List<CourseClass>();
            foreach (var classId in classIds)
            {
                var classInfo = await _classRepo.GetByIdAsync(classId);
                if (classInfo != null) classes.Add(classInfo);
            }

            for (int i = 0; i < classes.Count; i++)
            {
                for (int j = i + 1; j < classes.Count; j++)
                {
                    var first = classes[i];
                    var second = classes[j];

                    if (!HasDateOverlap(first.StartDate, first.EndDate, second.StartDate, second.EndDate))
                    {
                        continue;
                    }

                    if (HasScheduleOverlap(first.DayOfWeek, first.StartTime, first.EndTime,
                                           second.DayOfWeek, second.StartTime, second.EndTime))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool HasScheduleOverlap(string days1, TimeSpan start1, TimeSpan end1,
                                               string days2, TimeSpan start2, TimeSpan end2)
        {
            var daySet1 = days1.Split(',').Select(d => d.Trim()).ToHashSet();
            var daySet2 = days2.Split(',').Select(d => d.Trim()).ToHashSet();

            if (!daySet1.Intersect(daySet2).Any()) return false;

            return start1 < end2 && start2 < end1;
        }

        private static bool HasDateOverlap(DateTime? start1, DateTime? end1,
                                           DateTime? start2, DateTime? end2)
        {
            if (!start1.HasValue || !end1.HasValue || !start2.HasValue || !end2.HasValue)
            {
                return true;
            }

            return start1.Value.Date <= end2.Value.Date && start2.Value.Date <= end1.Value.Date;
        }

        public async Task<decimal> CalculateTotalFeeAsync(List<int> classIds)
        {
            decimal total = 0;

            foreach (var classId in classIds)
            {
                var classInfo = await _classRepo.GetByIdAsync(classId);
                if (classInfo != null)
                {
                    total += classInfo.Course.TuitionFee;
                }
            }

            return total;
        }

        public async Task<decimal> CalculateTotalFeeAsync(int studentId, List<int> classIds)
        {
            decimal total = 0;

            var pending = await _pendingRepo.GetPendingByStudentIdAsync(studentId);
            var pendingCourseIds = pending.Where(p => p.IsRetake).Select(p => p.CourseId).ToHashSet();

            foreach (var classId in classIds)
            {
                var classInfo = await _classRepo.GetByIdAsync(classId);
                if (classInfo != null)
                {
                    var fee = classInfo.Course.TuitionFee;
                    if (pendingCourseIds.Contains(classInfo.CourseId))
                    {
                        fee = Math.Round(fee * 0.5m, 0);
                    }

                    total += fee;
                }
            }

            return total;
        }

        public async Task<List<PendingCourseDto>> GetPendingCoursesAsync(int studentId)
        {
            var student = await _studentRepo.GetByIdWithDetailsAsync(studentId);
            if (student == null) return new List<PendingCourseDto>();

            var allCourses = await _courseRepo.GetCoursesByMajorAsync(student.MajorId);
            var publishedGrades = await _gradeRepo.GetPublishedGradesByStudentIdAsync(studentId);

            var failedCourseIds = publishedGrades
                .Where(g => !g.IsPassed)
                .Select(g => g.CourseId)
                .Distinct()
                .ToList();

            var openSemester = Math.Min(student.CurrentSemester + 1, 9);

            foreach (var courseId in failedCourseIds)
            {
                var course = allCourses.FirstOrDefault(c => c.CourseId == courseId);
                if (course == null) continue;

                var existing = await _pendingRepo.GetPendingAsync(studentId, courseId, openSemester);
                if (existing == null)
                {
                    await _pendingRepo.AddAsync(new PendingEnrollment
                    {
                        StudentId = studentId,
                        CourseId = courseId,
                        SemesterNumber = openSemester,
                        IsRetake = true,
                        RequiredFee = Math.Round(course.TuitionFee * 0.5m, 0),
                        Reason = "Failed course",
                        Status = "Pending",
                        CreatedDate = DateTime.Now
                    });
                }
            }

            await _pendingRepo.SaveChangesAsync();

            var pending = await _pendingRepo.GetPendingByStudentIdAsync(studentId);

            return pending.Select(p => new PendingCourseDto
            {
                CourseId = p.CourseId,
                CourseCode = p.Course.CourseCode,
                CourseName = p.Course.CourseName,
                RequiredFee = p.RequiredFee,
                Reason = p.Reason
            }).ToList();
        }

        public async Task<RetakeRegistrationDto> GetRetakeRegistrationDataAsync(int studentId)
        {
            var student = await _studentRepo.GetByIdWithDetailsAsync(studentId);
            if (student == null) throw new Exception("Student not found");

            var allCourses = await _courseRepo.GetCoursesByMajorAsync(student.MajorId);
            var pending = await _pendingRepo.GetPendingByStudentIdAsync(studentId);

            var retakePending = pending.Where(p => p.IsRetake).ToList();
            var retakeCourseIds = retakePending.Select(p => p.CourseId).Distinct().ToHashSet();

            var courses = allCourses.Where(c => retakeCourseIds.Contains(c.CourseId)).ToList();

            var resultCourses = new List<CourseWithClassesDto>();
            foreach (var course in courses)
            {
                var classes = await _classRepo.GetClassesByCourseAsync(course.CourseId);

                resultCourses.Add(new CourseWithClassesDto
                {
                    CourseId = course.CourseId,
                    CourseCode = course.CourseCode,
                    CourseName = course.CourseName,
                    Credits = course.Credits,
                    TuitionFee = course.TuitionFee,
                    IsEligible = true,
                    BlockReason = null,
                    PrerequisiteCourseId = course.PrerequisiteCourseId,
                    IsAlreadyRegistered = false,
                    RegisteredClassId = null,
                    RegisteredClassDisplay = null,
                    AvailableClasses = classes.Select(c => new ClassOptionDto
                    {
                        ClassId = c.ClassId,
                        ClassName = c.ClassName,
                        Schedule = c.Schedule,
                        DayOfWeek = c.DayOfWeek,
                        StartTime = c.StartTime,
                        EndTime = c.EndTime,
                        Room = c.Room,
                        InstructorName = c.Instructor?.FullName ?? "TBA",
                        CurrentEnrollment = c.CurrentEnrollment,
                        MaxStudents = c.MaxStudents,
                        StartDate = c.StartDate,
                        EndDate = c.EndDate
                    }).ToList()
                });
            }

            return new RetakeRegistrationDto
            {
                Courses = resultCourses,
                PendingCourses = retakePending.Select(p => new PendingCourseDto
                {
                    CourseId = p.CourseId,
                    CourseCode = p.Course.CourseCode,
                    CourseName = p.Course.CourseName,
                    RequiredFee = p.RequiredFee,
                    Reason = p.Reason
                }).ToList()
            };
        }
    }
}