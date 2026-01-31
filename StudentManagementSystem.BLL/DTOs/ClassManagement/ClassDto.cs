namespace StudentManagementSystem.BLL.DTOs
{
    public class ClassDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int? InstructorId { get; set; }
        public string InstructorName { get; set; } = string.Empty;
        public int SemesterId { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public string DayOfWeek { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public int CurrentEnrollment { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateClassRequest
    {
        public string ClassName { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int SemesterId { get; set; }
        public int? InstructorId { get; set; }
        public string Schedule { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public int MaxStudents { get; set; } = 45;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class UpdateClassRequest
    {
        public string ClassName { get; set; } = string.Empty;
        public int? InstructorId { get; set; }
        public string Schedule { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public int MaxStudents { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class ConflictCheckRequest
    {
        public int? InstructorId { get; set; }
        public int SemesterId { get; set; }
        public string Schedule { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public int? ExcludeClassId { get; set; }
    }

    public class ConflictCheckResult
    {
        public bool HasConflict { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string> Conflicts { get; set; } = new();
    }
}