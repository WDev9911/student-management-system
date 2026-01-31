using StudentManagementSystem.BLL.DTOs.Attendance;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IClassRepository _classRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IAttendanceRepository _attendanceRepo;

        public AttendanceService(
            IClassRepository classRepo,
            IEnrollmentRepository enrollmentRepo,
            IAttendanceRepository attendanceRepo)
        {
            _classRepo = classRepo;
            _enrollmentRepo = enrollmentRepo;
            _attendanceRepo = attendanceRepo;
        }

        public async Task<AttendanceSessionDto?> GetAttendanceSessionAsync(int classId, int instructorId, DateTime sessionDate)
        {
            var courseClass = await _classRepo.GetByIdAsync(classId);
            if (courseClass == null || courseClass.InstructorId != instructorId)
            {
                return null;
            }

            if (!IsClassOnDate(courseClass, sessionDate.Date))
            {
                return null;
            }

            var enrollments = await _enrollmentRepo.GetEnrollmentsByClassIdAsync(classId);
            var activeStudents = enrollments
                .Where(e => e.Status == "Active")
                .OrderBy(e => e.Student.StudentCode)
                .ToList();

            var session = await _attendanceRepo.GetSessionWithRecordsAsync(classId, sessionDate.Date);
            var recordMap = session?.Records.ToDictionary(r => r.StudentId, r => r.Status)
                           ?? new Dictionary<int, string>();

            return new AttendanceSessionDto
            {
                ClassId = courseClass.ClassId,
                ClassName = courseClass.ClassName,
                CourseCode = courseClass.Course.CourseCode,
                CourseName = courseClass.Course.CourseName,
                SessionDate = sessionDate.Date,
                Students = activeStudents.Select(s => new AttendanceStudentDto
                {
                    StudentId = s.StudentId,
                    StudentCode = s.Student.StudentCode,
                    FullName = s.Student.FullName,
                    Status = recordMap.TryGetValue(s.StudentId, out var status) ? status : "Present"
                }).ToList()
            };
        }

        private static bool IsClassOnDate(CourseClass cls, DateTime date)
        {
            if (cls.StartDate.HasValue && date.Date < cls.StartDate.Value.Date) return false;
            if (cls.EndDate.HasValue && date.Date > cls.EndDate.Value.Date) return false;

            var tokens = cls.DayOfWeek
                .Replace(" ", string.Empty)
                .Split(new[] { ',', ';', '|', '/', '-' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(t => t.ToUpperInvariant())
                .ToHashSet();

            var vnKey = date.DayOfWeek switch
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

            var enKey = date.DayOfWeek switch
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

        public async Task<bool> SaveAttendanceAsync(AttendanceSaveRequest request, int instructorId)
        {
            var courseClass = await _classRepo.GetByIdAsync(request.ClassId);
            if (courseClass == null || courseClass.InstructorId != instructorId)
            {
                return false;
            }

            var sessionDate = request.SessionDate.Date;
            var session = await _attendanceRepo.GetSessionAsync(request.ClassId, sessionDate);

            if (session == null)
            {
                session = new AttendanceSession
                {
                    ClassId = request.ClassId,
                    InstructorId = instructorId,
                    SessionDate = sessionDate,
                    CreatedAt = DateTime.Now
                };

                await _attendanceRepo.CreateSessionAsync(session);
                await _attendanceRepo.SaveChangesAsync();
            }

            var enrollments = await _enrollmentRepo.GetEnrollmentsByClassIdAsync(request.ClassId);
            var validStudentIds = enrollments
                .Where(e => e.Status == "Active")
                .Select(e => e.StudentId)
                .ToHashSet();

            var existingRecords = await _attendanceRepo.GetRecordsBySessionIdAsync(session.AttendanceSessionId);
            var recordMap = existingRecords.ToDictionary(r => r.StudentId);

            foreach (var item in request.Students.Where(s => validStudentIds.Contains(s.StudentId)))
            {
                var status = item.Status is "Present" or "Absent" ? item.Status : "Present";

                if (recordMap.TryGetValue(item.StudentId, out var record))
                {
                    record.Status = status;
                    record.UpdatedAt = DateTime.Now;
                }
                else
                {
                    existingRecords.Add(new AttendanceRecord
                    {
                        AttendanceSessionId = session.AttendanceSessionId,
                        StudentId = item.StudentId,
                        Status = status,
                        UpdatedAt = DateTime.Now
                    });
                }
            }

            var newRecords = existingRecords
                .Where(r => r.AttendanceRecordId == 0)
                .ToList();

            if (newRecords.Count > 0)
            {
                await _attendanceRepo.AddRecordsAsync(newRecords);
            }

            await _attendanceRepo.SaveChangesAsync();
            return true;
        }

        public async Task<List<StudentAttendanceDto>> GetStudentAttendanceAsync(int studentId, DateTime fromDate, DateTime toDate)
        {
            var records = await _attendanceRepo.GetRecordsByStudentAsync(studentId, fromDate, toDate);

            return records.Select(r => new StudentAttendanceDto
            {
                ClassId = r.AttendanceSession.CourseClass.ClassId,
                SessionDate = r.AttendanceSession.SessionDate,
                ClassName = r.AttendanceSession.CourseClass.ClassName,
                CourseCode = r.AttendanceSession.CourseClass.Course.CourseCode,
                CourseName = r.AttendanceSession.CourseClass.Course.CourseName,
                Status = r.Status
            }).ToList();
        }

        public async Task<List<StudentAttendanceClassDto>> GetStudentClassesAsync(int studentId)
        {
            var enrollments = await _enrollmentRepo.GetActiveEnrollmentsByStudentIdAsync(studentId);

            return enrollments
                .GroupBy(e => e.ClassId)
                .Select(g =>
                {
                    var e = g.First();
                    return new StudentAttendanceClassDto
                    {
                        ClassId = e.ClassId,
                        ClassName = e.CourseClass.ClassName,
                        CourseCode = e.Course.CourseCode,
                        CourseName = e.Course.CourseName
                    };
                })
                .OrderBy(c => c.ClassName)
                .ToList();
        }

        public async Task<List<InstructorAttendanceSessionDto>> GetInstructorSessionsAsync(int instructorId)
        {
            var sessions = await _attendanceRepo.GetSessionsByInstructorAsync(instructorId);

            return sessions.Select(s => new InstructorAttendanceSessionDto
            {
                ClassId = s.ClassId,
                SessionDate = s.SessionDate,
                ClassName = s.CourseClass.ClassName,
                CourseCode = s.CourseClass.Course.CourseCode,
                CourseName = s.CourseClass.Course.CourseName,
                PresentCount = s.Records.Count(r => r.Status == "Present"),
                AbsentCount = s.Records.Count(r => r.Status == "Absent"),
                TotalCount = s.Records.Count
            }).ToList();
        }
    }
}