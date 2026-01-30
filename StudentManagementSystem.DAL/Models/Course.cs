using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Môn học (108 môn cho 3 chuyên ngành)
    /// </summary>
    public class Course
    {
        [Key]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Mã môn học là bắt buộc")]
        [StringLength(20)]
        public string CourseCode { get; set; } = string.Empty; // IT101, IT201

        [Required(ErrorMessage = "Tên môn học là bắt buộc")]
        [StringLength(200)]
        public string CourseName { get; set; } = string.Empty;

        [Required]
        public int MajorId { get; set; }

        [ForeignKey("MajorId")]
        public virtual Major Major { get; set; } = null!;

        [Required]
        [Range(1, 9, ErrorMessage = "Kỳ học từ 1-9")]
        public int SemesterNumber { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Tín chỉ từ 1-5")]
        public int Credits { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 10000000, ErrorMessage = "Học phí không hợp lệ")]
        public decimal TuitionFee { get; set; }

        // Môn tiên quyết (Self-referencing)
        public int? PrerequisiteCourseId { get; set; }

        [ForeignKey("PrerequisiteCourseId")]
        public virtual Course? PrerequisiteCourse { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<CourseClass> CourseClasses { get; set; } = new List<CourseClass>();
        public virtual ICollection<InstructorAssignment> InstructorAssignments { get; set; } = new List<InstructorAssignment>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
    }
}