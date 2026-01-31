namespace StudentManagementSystem.BLL.DTOs.Attendance
{
    public class StudentAttendanceDto
    {
        public int ClassId { get; set; }
        public DateTime SessionDate { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string Status { get; set; } = "Present"; // Present | Absent
    }

    public class StudentAttendanceClassDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
    }
}