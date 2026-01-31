namespace StudentManagementSystem.BLL.DTOs.Attendance
{
    public class AttendanceSessionDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public DateTime SessionDate { get; set; }
        public List<AttendanceStudentDto> Students { get; set; } = new();
    }

    public class AttendanceStudentDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Status { get; set; } = "Present"; // Present | Absent
    }
}