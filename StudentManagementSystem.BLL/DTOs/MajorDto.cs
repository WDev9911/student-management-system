namespace StudentManagementSystem.BLL.DTOs
{
    public class MajorDto
    {
        public int MajorId { get; set; }
        public string MajorCode { get; set; } = string.Empty;
        public string MajorName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int DurationYears { get; set; }
        public int TotalSemesters { get; set; }
        public int TotalCredits { get; set; }
    }
}