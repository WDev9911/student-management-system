using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Điểm số của sinh viên
    /// </summary>
    public class Grade
    {
        [Key]
        public int GradeId { get; set; }

        [Required]
        public int EnrollmentId { get; set; }

        [ForeignKey("EnrollmentId")]
        public virtual Enrollment Enrollment { get; set; } = null!;

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;

        // Điểm thành phần (0-10 scale)
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 10)]
        public decimal AttendanceScore { get; set; } = 0; // Chuyên cần (10%)

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 10)]
        public decimal MidtermScore { get; set; } = 0; // Giữa kỳ (20%)

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 10)]
        public decimal AssignmentScore { get; set; } = 0; // Bài tập (20%)

        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 10)]
        public decimal FinalExamScore { get; set; } = 0; // Thi cuối kỳ (50%)

        // Điểm tổng kết
        [Column(TypeName = "decimal(5,2)")]
        [Range(0, 10)]
        public decimal FinalScore { get; set; } = 0; // = 0.1*Att + 0.2*Mid + 0.2*Ass + 0.5*Final

        [StringLength(5)]
        public string? LetterGrade { get; set; } // "A+", "A", "B+", "B", "C+", "C", "D+", "D", "F"

        [Column(TypeName = "decimal(3,2)")]
        [Range(0, 4)]
        public decimal GradePoint { get; set; } = 0; // 4.0 scale (A+=4.0, A=3.7, B+=3.3...)

        public bool IsPassed { get; set; } = false; // >= 5.0 = Pass

        public DateTime? GradedDate { get; set; } // Ngày nhập điểm

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}