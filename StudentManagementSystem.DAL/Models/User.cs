using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Tài khoản đăng nhập (3 roles: Student, Instructor, Admin)
    /// </summary>
    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Username là bắt buộc")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password là bắt buộc")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Role { get; set; } = string.Empty; // "Student", "Instructor", "Admin"

        // Link to Student or Instructor (nullable - vì Admin không có StudentId/InstructorId)
        public int? StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student? Student { get; set; }

        public int? InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public virtual Instructor? Instructor { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? LastLoginDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}