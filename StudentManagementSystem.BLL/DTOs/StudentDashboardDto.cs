namespace StudentManagementSystem.BLL.DTOs
{
    public class StudentDashboardDto
    {
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal WalletBalance { get; set; }
        public int EnrolledCoursesCount { get; set; }
        public decimal CurrentGPA { get; set; }
        public int CreditsCompleted { get; set; }
        public int CurrentSemester { get; set; }
        public int TotalCreditsRequired { get; set; }
        public List<RecentNotificationDto> RecentNotifications { get; set; } = new();
        public List<WeekScheduleDto> ThisWeekSchedule { get; set; } = new();
    }

    public class RecentNotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class WeekScheduleDto
    {
        public string DayOfWeek { get; set; } = string.Empty;
        public List<ClassSessionDto> Classes { get; set; } = new();
    }

    public class ClassSessionDto
    {
        public string CourseName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? RoomNumber { get; set; }
    }
}