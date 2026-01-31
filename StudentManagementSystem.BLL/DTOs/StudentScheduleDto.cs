namespace StudentManagementSystem.BLL.DTOs
{
    /// <summary>
    /// DTO chính cho màn hình th?i khóa bi?u
    /// </summary>
    public class StudentScheduleDto
    {
        public int TodaysClasses { get; set; }          // S? l?p h?c hôm nay
        public int TotalSessions { get; set; }          // T?ng s? bu?i h?c (3 tháng)
        public int OnlineClasses { get; set; }          // S? l?p h?c online
        public List<WeekScheduleDto> Weeks { get; set; } = new();
        public List<UpcomingClassDto> UpcomingClasses { get; set; } = new();
    }

    /// <summary>
    /// Thông tin 1 tu?n h?c
    /// </summary>
    public class WeekScheduleDto
    {
        public DateTime WeekStart { get; set; }
        public DateTime WeekEnd { get; set; }
        public string DisplayText => $"{WeekStart:MMM dd} - {WeekEnd:MMM dd, yyyy}";
        public List<DayScheduleDto> Days { get; set; } = new();
    }

    /// <summary>
    /// Thông tin 1 ngày trong tu?n
    /// </summary>
    public class DayScheduleDto
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; } = string.Empty;     // "MONDAY", "TUESDAY"
        public string DateDisplay { get; set; } = string.Empty; // "26/01"
        public bool IsToday { get; set; }
        public List<ClassSessionDto> Classes { get; set; } = new();
    }

    /// <summary>
    /// Thông tin 1 bu?i h?c
    /// </summary>
    public class ClassSessionDto
    {
        public int ClassId { get; set; }
        public string CourseCode { get; set; } = string.Empty;      // IT101
        public string CourseName { get; set; } = string.Empty;      // Nh?p môn l?p trình
        public string ClassName { get; set; } = string.Empty;       // IT101-A
        public TimeSpan StartTime { get; set; }                     // 07:00
        public TimeSpan EndTime { get; set; }                       // 09:00
        public string TimeDisplay => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
        public string Room { get; set; } = string.Empty;            // A101
        public string InstructorName { get; set; } = string.Empty;  // Nguy?n V?n A
        public string Color { get; set; } = "primary";              // Màu hi?n th?
        public string? AttendanceStatus { get; set; }               // Present | Absent | null
    }

    /// <summary>
    /// L?p h?c s?p di?n ra
    /// </summary>
    public class UpcomingClassDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string ClassName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTime ClassDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Room { get; set; } = string.Empty;
        public string TimeUntil { get; set; } = string.Empty;       // "Today", "Tomorrow"
    }
}