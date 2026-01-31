using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Lớp học (1 môn có nhiều lớp, vd: IT101-A, IT101-B)
    /// </summary>
    public class CourseClass
    {
        [Key]
        public int ClassId { get; set; }

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;

        public int? InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public virtual Instructor? Instructor { get; set; }

        [Required]
        public int SemesterId { get; set; }

        [ForeignKey("SemesterId")]
        public virtual Semester Semester { get; set; } = null!;

        [Required(ErrorMessage = "Tên lớp là bắt buộc")]
        [StringLength(50)]
        public string ClassName { get; set; } = string.Empty; // "IT101-A", "IT101-B"

        [Required(ErrorMessage = "Lịch học là bắt buộc")]
        [StringLength(100)]
        public string Schedule { get; set; } = string.Empty; // "T2,4 - 07:00-09:00"

        [Required]
        [StringLength(20)]
        public string DayOfWeek { get; set; } = string.Empty; // "T2,T4" parsed from Schedule

        [Required]
        public TimeSpan StartTime { get; set; } // 07:00

        [Required]
        public TimeSpan EndTime { get; set; } // 09:00

        [Required(ErrorMessage = "Phòng học là bắt buộc")]
        [StringLength(50)]
        public string Room { get; set; } = string.Empty; // "A101", "B205"

        [Range(20, 100, ErrorMessage = "Sĩ số từ 20-100")]
        public int MaxStudents { get; set; } = 45;

        public int CurrentEnrollment { get; set; } = 0; // Số sinh viên đã đăng ký

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Open"; // "Open", "Full", "Closed"

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();
    }
}