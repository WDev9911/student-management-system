using StudentManagementSystem.BLL.DTOs;
using StudentManagementSystem.BLL.Services.Interfaces;
using StudentManagementSystem.DAL.Models;
using StudentManagementSystem.DAL.Repositories.Interfaces;
using System.Text.RegularExpressions;

namespace StudentManagementSystem.BLL.Services.Implementations
{
    public class ClassService : IClassService
    {
        private readonly IClassRepository _classRepo;
        private readonly ICourseRepository _courseRepo;
        private readonly ISemesterRepository _semesterRepo;
        private readonly IInstructorRepository _instructorRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;

        public ClassService(
            IClassRepository classRepo,
            ICourseRepository courseRepo,
            ISemesterRepository semesterRepo,
            IInstructorRepository instructorRepo,
            IEnrollmentRepository enrollmentRepo)
        {
            _classRepo = classRepo;
            _courseRepo = courseRepo;
            _semesterRepo = semesterRepo;
            _instructorRepo = instructorRepo;
            _enrollmentRepo = enrollmentRepo;
        }

        public async Task<List<ClassDto>> GetAllClassesAsync()
        {
            var classes = await _classRepo.GetAllAsync();
            return classes.Select(MapToDto).ToList();
        }

        public async Task<ClassDto?> GetClassByIdAsync(int id)
        {
            var courseClass = await _classRepo.GetByIdAsync(id);
            return courseClass == null ? null : MapToDto(courseClass);
        }

        public async Task<List<ClassDto>> GetClassesByCourseAndSemesterAsync(int courseId, int semesterId)
        {
            var classes = await _classRepo.GetByCourseAndSemesterAsync(courseId, semesterId);
            return classes.Select(MapToDto).ToList();
        }

        public async Task<List<ClassDto>> GetClassesByInstructorAsync(int instructorId)
        {
            var classes = await _classRepo.GetByInstructorAsync(instructorId);
            return classes.Select(MapToDto).ToList();
        }

        public async Task<ClassDto> CreateClassAsync(CreateClassRequest request)
        {
            // 1. Validate class name format
            if (!Regex.IsMatch(request.ClassName, @"^[A-Z]{2,6}\d{3}-[A-Z]$"))
            {
                throw new FormatException("Invalid class code format. Example: IT101-A");
            }

            // 2. Check uniqueness
            if (await _classRepo.ClassNameExistsAsync(request.ClassName))
            {
                throw new InvalidOperationException($"Class '{request.ClassName}' already exists.");
            }

            // 3. Validate course
            var course = await _courseRepo.GetCourseByIdAsync(request.CourseId);
            if (course == null) throw new InvalidOperationException("Course not found.");

            // 4. Validate semester
            var semester = await _semesterRepo.GetSemesterByIdAsync(request.SemesterId);
            if (semester == null) throw new InvalidOperationException("Semester not found.");

            // 5. Validate instructor
            if (request.InstructorId.HasValue)
            {
                var instructor = await _instructorRepo.GetInstructorByIdAsync(request.InstructorId.Value);
                if (instructor == null) throw new InvalidOperationException("Instructor not found.");
            }

            // 6. Validate dates
            if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            {
                throw new InvalidOperationException("Class start and end dates are required.");
            }

            if (request.StartDate.Value.Date > request.EndDate.Value.Date)
            {
                throw new InvalidOperationException("Class start date must be before end date.");
            }

            // 7. Parse schedule
            var (dayOfWeek, startTime, endTime) = ParseSchedule(request.Schedule);

            // 8. Check conflicts
            var conflictCheck = await CheckConflictsAsync(new ConflictCheckRequest
            {
                InstructorId = request.InstructorId,
                SemesterId = request.SemesterId,
                Schedule = request.Schedule,
                Room = request.Room
            });

            if (conflictCheck.HasConflict)
            {
                throw new InvalidOperationException($"Schedule conflicts detected: {string.Join("; ", conflictCheck.Conflicts)}");
            }

            // 9. Create
            var courseClass = new CourseClass
            {
                ClassName = request.ClassName,
                CourseId = request.CourseId,
                InstructorId = request.InstructorId,
                SemesterId = request.SemesterId,
                Schedule = request.Schedule,
                DayOfWeek = dayOfWeek,
                StartTime = startTime,
                EndTime = endTime,
                Room = request.Room,
                MaxStudents = request.MaxStudents,
                CurrentEnrollment = 0,
                Status = "Open",
                StartDate = request.StartDate.Value.Date,
                EndDate = request.EndDate.Value.Date,
                CreatedDate = DateTime.Now
            };

            await _classRepo.CreateAsync(courseClass);
            return (await GetClassByIdAsync(courseClass.ClassId))!;
        }

        public async Task<ClassDto> UpdateClassAsync(int id, UpdateClassRequest request)
        {
            var courseClass = await _classRepo.GetByIdAsync(id);
            if (courseClass == null) throw new InvalidOperationException("Class not found.");

            if (!Regex.IsMatch(request.ClassName, @"^[A-Z]{2,6}\d{3}-[A-Z]$"))
            {
                throw new FormatException("Invalid class code format. Example: IT101-A");
            }

            if (await _classRepo.ClassNameExistsAsync(request.ClassName, id))
            {
                throw new InvalidOperationException($"Class '{request.ClassName}' already exists.");
            }

            if (request.InstructorId.HasValue)
            {
                var instructor = await _instructorRepo.GetInstructorByIdAsync(request.InstructorId.Value);
                if (instructor == null) throw new InvalidOperationException("Instructor not found.");
            }

            if (!request.StartDate.HasValue || !request.EndDate.HasValue)
            {
                throw new InvalidOperationException("Class start and end dates are required.");
            }

            if (request.StartDate.Value.Date > request.EndDate.Value.Date)
            {
                throw new InvalidOperationException("Class start date must be before end date.");
            }

            var (dayOfWeek, startTime, endTime) = ParseSchedule(request.Schedule);

            var conflictCheck = await CheckConflictsAsync(new ConflictCheckRequest
            {
                InstructorId = request.InstructorId,
                SemesterId = courseClass.SemesterId,
                Schedule = request.Schedule,
                Room = request.Room,
                ExcludeClassId = id
            });

            if (conflictCheck.HasConflict)
            {
                throw new InvalidOperationException($"Schedule conflicts detected: {string.Join("; ", conflictCheck.Conflicts)}");
            }

            courseClass.ClassName = request.ClassName;
            courseClass.InstructorId = request.InstructorId;
            courseClass.Schedule = request.Schedule;
            courseClass.DayOfWeek = dayOfWeek;
            courseClass.StartTime = startTime;
            courseClass.EndTime = endTime;
            courseClass.Room = request.Room;
            courseClass.MaxStudents = request.MaxStudents;
            courseClass.Status = request.Status;
            courseClass.StartDate = request.StartDate.Value.Date;
            courseClass.EndDate = request.EndDate.Value.Date;

            await _classRepo.UpdateAsync(courseClass);
            return (await GetClassByIdAsync(id))!;
        }

        public async Task<bool> DeleteClassAsync(int id)
        {
            var courseClass = await _classRepo.GetByIdAsync(id);
            if (courseClass == null) return false;

            if (courseClass.CurrentEnrollment > 0)
            {
                throw new InvalidOperationException("Cannot delete a class with enrolled students.");
            }

            return await _classRepo.DeleteAsync(id);
        }

        public async Task<ConflictCheckResult> CheckConflictsAsync(ConflictCheckRequest request)
        {
            var result = new ConflictCheckResult { HasConflict = false };

            try
            {
                var (dayOfWeek, startTime, endTime) = ParseSchedule(request.Schedule);

                // Check instructor
                if (request.InstructorId.HasValue)
                {
                    var instructorClasses = await _classRepo.GetByInstructorAndSemesterAsync(
                        request.InstructorId.Value, request.SemesterId);

                    if (request.ExcludeClassId.HasValue)
                    {
                        instructorClasses = instructorClasses.Where(c => c.ClassId != request.ExcludeClassId.Value).ToList();
                    }

                    foreach (var cls in instructorClasses)
                    {
                        if (HasScheduleOverlap(dayOfWeek, startTime, endTime, cls.DayOfWeek, cls.StartTime, cls.EndTime))
                        {
                            result.HasConflict = true;
                            result.Conflicts.Add($"Instructor {cls.Instructor?.FullName} is already teaching {cls.ClassName} ({cls.Schedule})");
                        }
                    }
                }

                // Check room
                if (!string.IsNullOrWhiteSpace(request.Room))
                {
                    var roomClasses = await _classRepo.GetByRoomAndSemesterAsync(request.Room, request.SemesterId);

                    if (request.ExcludeClassId.HasValue)
                    {
                        roomClasses = roomClasses.Where(c => c.ClassId != request.ExcludeClassId.Value).ToList();
                    }

                    foreach (var cls in roomClasses)
                    {
                        if (HasScheduleOverlap(dayOfWeek, startTime, endTime, cls.DayOfWeek, cls.StartTime, cls.EndTime))
                        {
                            result.HasConflict = true;
                            result.Conflicts.Add($"Room {request.Room} is occupied by {cls.ClassName} ({cls.Schedule})");
                        }
                    }
                }

                result.Message = result.HasConflict ? "Conflicts detected" : "No conflicts";
            }
            catch (Exception ex)
            {
                result.HasConflict = true;
                result.Message = "Error checking conflicts";
                result.Conflicts.Add(ex.Message);
            }

            return result;
        }

        public async Task<ClassStudentsDto?> GetClassStudentsAsync(int classId, int instructorId)
        {
            var courseClass = await _classRepo.GetByIdAsync(classId);
            if (courseClass == null || courseClass.InstructorId != instructorId)
            {
                return null;
            }

            var enrollments = await _enrollmentRepo.GetEnrollmentsByClassIdAsync(classId);

            var students = enrollments.Select(e => new ClassStudentItemDto
            {
                StudentId = e.StudentId,
                StudentCode = e.Student.StudentCode,
                FullName = e.Student.FullName,
                Email = e.Student.Email,
                Phone = e.Student.Phone,
                Status = e.Status,
                PaidAmount = e.PaidAmount,
                EnrollmentDate = e.EnrollmentDate
            }).ToList();

            return new ClassStudentsDto
            {
                ClassId = courseClass.ClassId,
                ClassName = courseClass.ClassName,
                CourseCode = courseClass.Course.CourseCode,
                CourseName = courseClass.Course.CourseName,
                SemesterName = courseClass.Semester.SemesterName,
                Schedule = courseClass.Schedule,
                Room = courseClass.Room,
                CurrentEnrollment = courseClass.CurrentEnrollment,
                MaxStudents = courseClass.MaxStudents,
                ActiveStudents = students.Count(s => s.Status == "Active"),
                DroppedStudents = students.Count(s => s.Status == "Dropped"),
                PaidStudents = students.Count(s => s.PaidAmount > 0),
                Students = students
            };
        }

        // HELPERS
        private ClassDto MapToDto(CourseClass c)
        {
            return new ClassDto
            {
                ClassId = c.ClassId,
                ClassName = c.ClassName,
                CourseId = c.CourseId,
                CourseCode = c.Course.CourseCode,
                CourseName = c.Course.CourseName,
                InstructorId = c.InstructorId,
                InstructorName = c.Instructor?.FullName ?? "Not assigned",
                SemesterId = c.SemesterId,
                SemesterName = c.Semester.SemesterName,
                Schedule = c.Schedule,
                DayOfWeek = c.DayOfWeek,
                StartTime = c.StartTime,
                EndTime = c.EndTime,
                Room = c.Room,
                MaxStudents = c.MaxStudents,
                CurrentEnrollment = c.CurrentEnrollment,
                Status = c.Status,
                StartDate = c.StartDate,
                EndDate = c.EndDate,
                CreatedDate = c.CreatedDate
            };
        }

        private (string dayOfWeek, TimeSpan startTime, TimeSpan endTime) ParseSchedule(string schedule)
        {
            // Support both English format (Mon,Wed - 07:00-09:00) and Vietnamese format (T2,4 - 07:00-09:00)
            var englishPattern = @"^(Mon|Tue|Wed|Thu|Fri|Sat|Sun)(?:,(Mon|Tue|Wed|Thu|Fri|Sat|Sun))*\s*-\s*(\d{2}):(\d{2})-(\d{2}):(\d{2})$";
            var vietnamesePattern = @"^(T[2-7](?:,T[2-7])*)\s*-\s*(\d{2}):(\d{2})-(\d{2}):(\d{2})$";

            var englishMatch = Regex.Match(schedule.Trim(), englishPattern);
            var vietnameseMatch = Regex.Match(schedule.Trim(), vietnamesePattern);

            string dayOfWeek;
            int startHour, startMinute, endHour, endMinute;

            if (englishMatch.Success)
            {
                // Extract days from English format
                dayOfWeek = ExtractDaysFromEnglish(schedule.Split('-')[0].Trim());
                startHour = int.Parse(englishMatch.Groups[3].Value);
                startMinute = int.Parse(englishMatch.Groups[4].Value);
                endHour = int.Parse(englishMatch.Groups[5].Value);
                endMinute = int.Parse(englishMatch.Groups[6].Value);
            }
            else if (vietnameseMatch.Success)
            {
                // Vietnamese format
                dayOfWeek = vietnameseMatch.Groups[1].Value;
                startHour = int.Parse(vietnameseMatch.Groups[2].Value);
                startMinute = int.Parse(vietnameseMatch.Groups[3].Value);
                endHour = int.Parse(vietnameseMatch.Groups[4].Value);
                endMinute = int.Parse(vietnameseMatch.Groups[5].Value);
            }
            else
            {
                throw new FormatException("Invalid schedule format. Example: Mon,Wed - 07:00-09:00 or T2,4 - 07:00-09:00");
            }

            var startTime = new TimeSpan(startHour, startMinute, 0);
            var endTime = new TimeSpan(endHour, endMinute, 0);

            if (startTime >= endTime)
            {
                throw new FormatException("Start time must be earlier than end time.");
            }

            if (startHour < 7 || endHour > 21)
            {
                throw new FormatException("Class hours must be between 07:00 and 21:00.");
            }

            return (dayOfWeek, startTime, endTime);
        }

        private string ExtractDaysFromEnglish(string daysStr)
        {
            // Convert English day names to Vietnamese format for storage
            var dayMap = new Dictionary<string, string>
            {
                { "Mon", "T2" },
                { "Tue", "T3" },
                { "Wed", "T4" },
                { "Thu", "T5" },
                { "Fri", "T6" },
                { "Sat", "T7" },
                { "Sun", "CN" }
            };

            var days = daysStr.Split(',').Select(d => d.Trim()).ToList();
            var convertedDays = days.Select(d => dayMap.ContainsKey(d) ? dayMap[d] : d);
            return string.Join(",", convertedDays);
        }

        private bool HasScheduleOverlap(string days1, TimeSpan start1, TimeSpan end1,
                                        string days2, TimeSpan start2, TimeSpan end2)
        {
            var daySet1 = days1.Split(',').Select(d => d.Trim()).ToHashSet();
            var daySet2 = days2.Split(',').Select(d => d.Trim()).ToHashSet();

            // Check if any days overlap
            if (!daySet1.Intersect(daySet2).Any()) return false;

            // Check if time ranges overlap
            return start1 < end2 && start2 < end1;
        }
    }
}