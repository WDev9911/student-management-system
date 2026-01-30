using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Đăng ký môn học của sinh viên
    /// </summary>
    public class Enrollment
    {
        [Key]
        public int EnrollmentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        [Required]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual CourseClass CourseClass { get; set; } = null!;

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;

        [Required]
        public int SemesterNumber { get; set; } // Kỳ đăng ký (1-9)

        public DateTime EnrollmentDate { get; set; } = DateTime.Now;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // "Active", "Dropped", "Completed"

        public bool IsRetake { get; set; } = false; // True = Học lại môn rớt

        [Column(TypeName = "decimal(18,2)")]
        public decimal PaidAmount { get; set; } = 0; // Số tiền đã trả

        public DateTime? DroppedDate { get; set; } // Ngày hủy đăng ký

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Grade? Grade { get; set; }
    }
}