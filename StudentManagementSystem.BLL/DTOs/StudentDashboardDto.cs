namespace StudentManagementSystem.BLL.DTOs
{
    public class StudentDashboardDto
    {
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal WalletBalance { get; set; }
        public int EnrolledCoursesCount { get; set; }

        public decimal SemesterGPA { get; set; }
        public decimal CumulativeGPA { get; set; }
        public decimal CurrentGPA { get; set; }

        public int CreditsCompleted { get; set; }
        public int CurrentSemester { get; set; }
        public int TotalCreditsRequired { get; set; }
        public List<RecentNotificationDto> RecentNotifications { get; set; } = new();
        public List<DashboardWeekScheduleDto> ThisWeekSchedule { get; set; } = new();
    }

    public class RecentNotificationDto
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
    }

    public class DashboardWeekScheduleDto
    {
        public string DayOfWeek { get; set; } = string.Empty;
        public List<DashboardClassSessionDto> Classes { get; set; } = new();
    }

    public class DashboardClassSessionDto
    {
        public string CourseName { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? RoomNumber { get; set; }
    }
}