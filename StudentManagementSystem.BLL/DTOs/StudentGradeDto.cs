namespace StudentManagementSystem.BLL.DTOs
{
    public class StudentGradeDto
    {
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int SemesterNumber { get; set; }
        public decimal QuizScore { get; set; }
        public decimal MidtermScore { get; set; }
        public decimal AssignmentScore { get; set; }
        public decimal FinalExamScore { get; set; }
        public decimal FinalScore { get; set; }
        public bool IsPassed { get; set; }
        public DateTime? GradedDate { get; set; }
    }
}