namespace StudentManagementSystem.BLL.DTOs
{
    public class ClassStudentsDto
    {
        public int ClassId { get; set; }
        public string ClassName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public string SemesterName { get; set; } = string.Empty;
        public string Schedule { get; set; } = string.Empty;
        public string Room { get; set; } = string.Empty;
        public int CurrentEnrollment { get; set; }
        public int MaxStudents { get; set; }

        public int ActiveStudents { get; set; }
        public int DroppedStudents { get; set; }
        public int PaidStudents { get; set; }

        public List<ClassStudentItemDto> Students { get; set; } = new();
    }

    public class ClassStudentItemDto
    {
        public int StudentId { get; set; }
        public string StudentCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string Status { get; set; } = string.Empty; // Active/Dropped
        public decimal PaidAmount { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}