using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Giảng viên
    /// </summary>
    public class Instructor
    {
        [Key]
        public int InstructorId { get; set; }

        [Required(ErrorMessage = "Mã giảng viên là bắt buộc")]
        [StringLength(20)]
        public string InstructorCode { get; set; } = string.Empty; // GV001, GV002

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Department { get; set; } // Khoa: "Computer Science", "Information Technology"

        [StringLength(200)]
        public string? Specialization { get; set; } // Chuyên môn: "Database", "Programming"

        [StringLength(500)]
        public string? Qualifications { get; set; } // Trình độ: "Thạc sĩ CNTT", "Tiến sĩ"

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Inactive

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual User? User { get; set; }
        public virtual ICollection<InstructorAssignment> InstructorAssignments { get; set; } = new List<InstructorAssignment>();
        public virtual ICollection<CourseClass> CourseClasses { get; set; } = new List<CourseClass>();
        public virtual ICollection<AttendanceSession> AttendanceSessions { get; set; } = new List<AttendanceSession>();
    }
}