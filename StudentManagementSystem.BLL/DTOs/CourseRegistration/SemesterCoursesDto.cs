namespace StudentManagementSystem.BLL.DTOs.CourseRegistration
{
    public class SemesterCoursesDto
    {
        public int SemesterNumber { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public bool IsCurrentSemester { get; set; }
        public bool CanRegister { get; set; }
        public bool IsRegistrationLocked { get; set; }
        public bool HasRegisteredCourses { get; set; }
        public decimal RegisteredTotalFee { get; set; }
        public List<RegisteredCourseDto> RegisteredCourses { get; set; } = new();
        public List<CourseWithClassesDto> Courses { get; set; } = new();
        public List<PendingCourseDto> PendingCourses { get; set; } = new();
    }

    public class CourseWithClassesDto
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public decimal TuitionFee { get; set; }
        public bool IsEligible { get; set; }
        public string? BlockReason { get; set; }
        public int? PrerequisiteCourseId { get; set; }
        public string? PrerequisiteCourseName { get; set; }
        public bool IsAlreadyRegistered { get; set; }
        public int? RegisteredClassId { get; set; }
        public string? RegisteredClassDisplay { get; set; }
        public List<ClassOptionDto> AvailableClasses { get; set; } = new();
    }

    public class RegisteredCourseDto
    {
        public int ClassId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public decimal TuitionFee { get; set; }
    }

    public class ClassOptionDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;
        public string InstructorName { get; set; } = string.Empty;
        public int CurrentEnrollment { get; set; }
        public int MaxStudents { get; set; }
        public bool IsFull => CurrentEnrollment >= MaxStudents;

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class EnrollmentRequestDto
    {
        public int StudentId { get; set; }
        public List<int> SelectedClassIds { get; set; } = new();
    }

    public class EnrollmentResultDto
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public decimal TotalFee { get; set; }
        public decimal RemainingBalance { get; set; }
        public List<string> EnrolledCourses { get; set; } = new();
        public List<string> Errors { get; set; } = new();
    }

    public class PendingCourseDto
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public decimal RequiredFee { get; set; }
        public string? Reason { get; set; }
    }

    public class RetakeRegistrationDto
    {
        public List<CourseWithClassesDto> Courses { get; set; } = new();
        public List<PendingCourseDto> PendingCourses { get; set; } = new();
    }
}