namespace StudentManagementSystem.BLL.DTOs
{
    public class SemesterDto
    {
        public int SemesterId { get; set; }
        public int SemesterNumber { get; set; }
        public string SemesterName { get; set; } = string.Empty;
        public string AcademicYear { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}