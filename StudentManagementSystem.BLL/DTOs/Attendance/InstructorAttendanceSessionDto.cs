namespace StudentManagementSystem.BLL.DTOs.Attendance
{
    public class InstructorAttendanceSessionDto
    {
        public int ClassId { get; set; }
        public DateTime SessionDate { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int PresentCount { get; set; }
        public int AbsentCount { get; set; }
        public int TotalCount { get; set; }
    }
}