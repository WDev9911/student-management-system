namespace StudentManagementSystem.BLL.DTOs
{
    public class GradeEntryDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public List<StudentGradeEntryDto> Students { get; set; } = new();
    }

    public class StudentGradeEntryDto
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public decimal QuizScore { get; set; }
        public decimal MidtermScore { get; set; }
        public decimal AssignmentScore { get; set; }
        public decimal FinalExamScore { get; set; }
        public decimal FinalScore { get; set; }
        public bool IsPassed { get; set; }
    }
}