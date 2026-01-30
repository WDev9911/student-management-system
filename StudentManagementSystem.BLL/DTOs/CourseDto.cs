namespace StudentManagementSystem.BLL.DTOs
{
    public class CourseDto
    {
        public int CourseId { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int Credits { get; set; }
        public decimal TuitionFee { get; set; }
        public string? Description { get; set; }
        public int MajorId { get; set; }
        public string MajorName { get; set; } = string.Empty;
        public int SemesterNumber { get; set; }
        public int? PrerequisiteCourseId { get; set; }
        public string? PrerequisiteCourseName { get; set; }
    }
}