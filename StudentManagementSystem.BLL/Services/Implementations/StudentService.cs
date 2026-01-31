using Microsoft.EntityFrameworkCore;
using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IWalletRepository _walletRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IGradeRepository _gradeRepo;
        private readonly IAttendanceRepository _attendanceRepo;

        private static readonly string[] ScheduleColors = { "primary", "warning", "success", "info", "danger" };

        public StudentService(
            IStudentRepository studentRepo,
            IWalletRepository walletRepo,
            IEnrollmentRepository enrollmentRepo,
            IGradeRepository gradeRepo,
            IAttendanceRepository attendanceRepo)
        {
            _studentRepo = studentRepo;
            _walletRepo = walletRepo;
            _enrollmentRepo = enrollmentRepo;
            _gradeRepo = gradeRepo;
            _attendanceRepo = attendanceRepo;
        }

        public async Task<StudentDashboardDto?> GetDashboardDataAsync(int studentId)
        {
            var student = await _studentRepo.GetByIdWithDetailsAsync(studentId);
            if (student == null) return null;

            var wallet = await _walletRepo.GetByStudentIdAsync(studentId);
            var today = DateTime.Today;

            var enrollments = await _enrollmentRepo.GetActiveEnrollmentsByStudentIdAsync(studentId);
            var thisWeekSchedule = BuildDashboardWeekSchedule(enrollments, today);

            var publishedGrades = await _gradeRepo.GetPublishedGradesByStudentIdAsync(studentId);
            var semesterGrades = publishedGrades
                .Where(g => g.Enrollment.SemesterNumber == student.CurrentSemester)
                .ToList();

            var semesterGpa = CalculateGpa(semesterGrades);
            var cumulativeGpa = CalculateGpa(publishedGrades);

            var creditsCompleted = publishedGrades
                .Where(g => g.IsPassed)
                .Sum(g => g.Course.Credits);

            return new StudentDashboardDto
            {
                StudentCode = student.StudentCode,
                FullName = student.FullName,
                WalletBalance = wallet?.Balance ?? 0,
                EnrolledCoursesCount = student.Enrollments.Count(e => e.Status == "Active"),
                SemesterGPA = semesterGpa,
                CumulativeGPA = cumulativeGpa,
                CurrentGPA = cumulativeGpa,
                CreditsCompleted = creditsCompleted,
                CurrentSemester = student.CurrentSemester,
                TotalCreditsRequired = student.Major?.TotalCredits ?? 0,
                ThisWeekSchedule = thisWeekSchedule
            };
        }

        public async Task<WalletInfoDto?> GetWalletInfoAsync(int studentId)
        {
            var wallet = await _walletRepo.GetByStudentIdAsync(studentId);
            if (wallet == null) return null;

            return new WalletInfoDto
            {
                Balance = wallet.Balance
            };
        }

        /// <summary>
        /// Th?i khóa bi?u 3 tháng (12 tu?n)
        /// </summary>
        public async Task<StudentScheduleDto> GetStudentScheduleAsync(int studentId)
        {
            var today = DateTime.Today;

            var enrollments = await _enrollmentRepo.GetActiveEnrollmentsByStudentIdAsync(studentId);
            var colorMap = new Dictionary<string, string>();

            var minStart = enrollments
                .Select(e => e.CourseClass.StartDate)
                .Where(d => d.HasValue)
                .Min();

            var maxEnd = enrollments
                .Select(e => e.CourseClass.EndDate)
                .Where(d => d.HasValue)
                .Max();

            var startDate = GetMonday(minStart?.Date ?? today);
            var endDate = maxEnd?.Date ?? startDate.AddDays(7 * 11);

            var attendanceRecords = await _attendanceRepo.GetRecordsByStudentAsync(studentId, startDate, endDate);
            var attendanceMap = attendanceRecords.ToDictionary(
                r => (r.AttendanceSession.ClassId, r.AttendanceSession.SessionDate.Date),
                r => r.Status);

            var totalWeeks = (int)Math.Ceiling((endDate - startDate).TotalDays / 7.0) + 1;

            var weeks = new List<WeekScheduleDto>();
            for (int weekNum = 0; weekNum < totalWeeks; weekNum++)
            {
                var weekStart = startDate.AddDays(weekNum * 7);
                var week = CreateWeekSchedule(enrollments, weekStart, colorMap, attendanceMap);
                weeks.Add(week);
            }

            var allClasses = weeks.SelectMany(w => w.Days).SelectMany(d => d.Classes).ToList();
            var todaysClasses = weeks
                .SelectMany(w => w.Days)
                .FirstOrDefault(d => d.Date.Date == today)?
                .Classes.Count ?? 0;

            var upcomingClasses = GetUpcomingClassesList(weeks, today);

            return new StudentScheduleDto
            {
                TodaysClasses = todaysClasses,
                TotalSessions = allClasses.Count,
                OnlineClasses = 0,
                Weeks = weeks,
                UpcomingClasses = upcomingClasses
            };
        }

        private WeekScheduleDto CreateWeekSchedule(
            List<Enrollment> enrollments,
            DateTime weekStart,
            Dictionary<string, string> colorMap,
            Dictionary<(int classId, DateTime date), string> attendanceMap)
        {
            var days = new List<DayScheduleDto>();

            for (int i = 0; i < 7; i++)
            {
                var currentDate = weekStart.AddDays(i);
                var day = CreateDaySchedule(enrollments, currentDate, colorMap, attendanceMap);
                days.Add(day);
            }

            return new WeekScheduleDto
            {
                WeekStart = weekStart,
                WeekEnd = weekStart.AddDays(6),
                Days = days
            };
        }

        private DayScheduleDto CreateDaySchedule(
            List<Enrollment> enrollments,
            DateTime date,
            Dictionary<string, string> colorMap,
            Dictionary<(int classId, DateTime date), string> attendanceMap)
        {
            var classes = new List<ClassSessionDto>();

            foreach (var enrollment in enrollments)
            {
                var cls = enrollment.CourseClass;

                if (cls.StartDate.HasValue && date.Date < cls.StartDate.Value.Date) continue;
                if (cls.EndDate.HasValue && date.Date > cls.EndDate.Value.Date) continue;

                if (!IsClassOnDay(cls.DayOfWeek, date.DayOfWeek))
                {
                    continue;
                }

                attendanceMap.TryGetValue((enrollment.ClassId, date.Date), out var status);

                classes.Add(new ClassSessionDto
                {
                    ClassId = enrollment.ClassId,
                    CourseCode = enrollment.Course.CourseCode,
                    CourseName = enrollment.Course.CourseName,
                    ClassName = cls.ClassName,
                    StartTime = cls.StartTime,
                    EndTime = cls.EndTime,
                    Room = cls.Room,
                    InstructorName = cls.Instructor?.FullName ?? "TBA",
                    Color = GetColorForCourse(colorMap, enrollment.Course.CourseCode),
                    AttendanceStatus = status
                });
            }

            return new DayScheduleDto
            {
                Date = date,
                DayName = GetDayName(date.DayOfWeek),
                DateDisplay = date.ToString("dd/MM"),
                IsToday = date.Date == DateTime.Today,
                Classes = classes.OrderBy(c => c.StartTime).ToList()
            };
        }

        private List<UpcomingClassDto> GetUpcomingClassesList(
            List<WeekScheduleDto> weeks,
            DateTime today)
        {
            var upcomingClasses = new List<UpcomingClassDto>();

            foreach (var week in weeks.Take(2))
            {
                foreach (var day in week.Days)
                {
                    if (day.Date < today) continue;

                    foreach (var classSession in day.Classes)
                    {
                        var daysUntil = (day.Date - today).Days;
                        var timeUntil = daysUntil switch
                        {
                            0 => "Today",
                            1 => "Tomorrow",
                            _ => $"In {daysUntil} days"
                        };

                        upcomingClasses.Add(new UpcomingClassDto
                        {
                            CourseCode = classSession.CourseCode,
                            ClassName = classSession.ClassName,
                            CourseName = classSession.CourseName,
                            ClassDate = day.Date,
                            StartTime = classSession.StartTime,
                            EndTime = classSession.EndTime,
                            Room = classSession.Room,
                            TimeUntil = timeUntil
                        });
                    }
                }
            }

            return upcomingClasses
                .OrderBy(c => c.ClassDate)
                .ThenBy(c => c.StartTime)
                .Take(5)
                .ToList();
        }

        private async Task<List<ClassSessionDto>> GetTodayClassesAsync(int studentId, DateTime today)
        {
            var enrollments = await _enrollmentRepo.GetActiveEnrollmentsByStudentIdAsync(studentId);
            var colorMap = new Dictionary<string, string>();

            var attendanceRecords = await _attendanceRepo.GetRecordsByStudentAsync(studentId, today.Date, today.Date);
            var attendanceMap = attendanceRecords.ToDictionary(
                r => (r.AttendanceSession.ClassId, r.AttendanceSession.SessionDate.Date),
                r => r.Status);

            return CreateDaySchedule(enrollments, today, colorMap, attendanceMap).Classes;
        }

        private static string GetColorForCourse(Dictionary<string, string> colorMap, string courseCode)
        {
            if (colorMap.TryGetValue(courseCode, out var color))
            {
                return color;
            }

            color = ScheduleColors[colorMap.Count % ScheduleColors.Length];
            colorMap[courseCode] = color;
            return color;
        }

        private static bool IsClassOnDay(string dayOfWeekText, DayOfWeek dayOfWeek)
        {
            var tokens = dayOfWeekText
                .Replace(" ", string.Empty)
                .Split(new[] { ',', ';', '|', '/', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToUpperInvariant())
                .ToHashSet();

            var vnKey = dayOfWeek switch
            {
                DayOfWeek.Monday => "T2",
                DayOfWeek.Tuesday => "T3",
                DayOfWeek.Wednesday => "T4",
                DayOfWeek.Thursday => "T5",
                DayOfWeek.Friday => "T6",
                DayOfWeek.Saturday => "T7",
                DayOfWeek.Sunday => "CN",
                _ => ""
            };

            var enKey = dayOfWeek switch
            {
                DayOfWeek.Monday => "MON",
                DayOfWeek.Tuesday => "TUE",
                DayOfWeek.Wednesday => "WED",
                DayOfWeek.Thursday => "THU",
                DayOfWeek.Friday => "FRI",
                DayOfWeek.Saturday => "SAT",
                DayOfWeek.Sunday => "SUN",
                _ => ""
            };

            return tokens.Contains(vnKey) || tokens.Contains(enKey);
        }

        private static string GetDayKeyForDate(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "T2",
                DayOfWeek.Tuesday => "T3",
                DayOfWeek.Wednesday => "T4",
                DayOfWeek.Thursday => "T5",
                DayOfWeek.Friday => "T6",
                DayOfWeek.Saturday => "T7",
                DayOfWeek.Sunday => "CN",
                _ => ""
            };
        }

        private static string GetDayName(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "MONDAY",
                DayOfWeek.Tuesday => "TUESDAY",
                DayOfWeek.Wednesday => "WEDNESDAY",
                DayOfWeek.Thursday => "THURSDAY",
                DayOfWeek.Friday => "FRIDAY",
                DayOfWeek.Saturday => "SATURDAY",
                DayOfWeek.Sunday => "SUNDAY",
                _ => ""
            };
        }

        private static string GetDayShortName(DayOfWeek dayOfWeek)
        {
            return dayOfWeek switch
            {
                DayOfWeek.Monday => "Mon",
                DayOfWeek.Tuesday => "Tue",
                DayOfWeek.Wednesday => "Wed",
                DayOfWeek.Thursday => "Thu",
                DayOfWeek.Friday => "Fri",
                DayOfWeek.Saturday => "Sat",
                DayOfWeek.Sunday => "Sun",
                _ => ""
            };
        }

        private static DateTime GetMonday(DateTime date)
        {
            int daysFromMonday = ((int)date.DayOfWeek - (int)DayOfWeek.Monday + 7) % 7;
            return date.AddDays(-daysFromMonday);
        }

        private static List<DashboardWeekScheduleDto> BuildDashboardWeekSchedule(
            List<Enrollment> enrollments,
            DateTime today)
        {
            var monday = GetMonday(today);
            var result = new List<DashboardWeekScheduleDto>();

            for (int i = 0; i < 7; i++)
            {
                var date = monday.AddDays(i);
                var dayName = GetDayName(date.DayOfWeek);

                var classes = enrollments
                    .Where(e => IsClassOnDay(e.CourseClass.DayOfWeek, date.DayOfWeek))
                    .Select(e => new DashboardClassSessionDto
                    {
                        CourseName = e.Course.CourseName,
                        StartTime = e.CourseClass.StartTime,
                        EndTime = e.CourseClass.EndTime,
                        RoomNumber = e.CourseClass.Room
                    })
                    .OrderBy(c => c.StartTime)
                    .ToList();

                result.Add(new DashboardWeekScheduleDto
                {
                    DayOfWeek = dayName,
                    Classes = classes
                });
            }

            return result;
        }

        public async Task<List<StudentGradeDto>> GetPublishedGradesAsync(int studentId)
        {
            var grades = await _gradeRepo.GetPublishedGradesByStudentIdAsync(studentId);

            return grades.Select(g => new StudentGradeDto
            {
                CourseCode = g.Course.CourseCode,
                CourseName = g.Course.CourseName,
                SemesterNumber = g.Enrollment.SemesterNumber,
                QuizScore = g.QuizScore,
                MidtermScore = g.MidtermScore,
                AssignmentScore = g.AssignmentScore,
                FinalExamScore = g.FinalExamScore,
                FinalScore = g.FinalScore,
                IsPassed = g.IsPassed,
                GradedDate = g.GradedDate
            }).ToList();
        }

        private static decimal CalculateGpa(List<Grade> grades)
        {
            if (grades.Count == 0) return 0;

            var totalCredits = grades.Sum(g => g.Course.Credits);
            if (totalCredits == 0) return 0;

            var totalPoints = grades.Sum(g =>
            {
                var gradePoint = Math.Round((g.FinalScore / 10m) * 4m, 2);
                return gradePoint * g.Course.Credits;
            });

            return Math.Round(totalPoints / totalCredits, 2);
        }
    }
}