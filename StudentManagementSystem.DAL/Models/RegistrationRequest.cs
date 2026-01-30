using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Yêu cầu đăng ký tài khoản sinh viên (chờ Admin duyệt)
    /// </summary>
    public class RegistrationRequest
    {
        [Key]
        public int RequestId { get; set; }

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

        [Required]
        public DateTime DateOfBirth { get; set; }

        [StringLength(10)]
        public string? Gender { get; set; }

        [StringLength(500)]
        public string? Address { get; set; }

        [Required]
        public int MajorId { get; set; }

        [Required(ErrorMessage = "Username là bắt buộc")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password là bắt buộc")]
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // "Pending", "Approved", "Rejected"

        [StringLength(500)]
        public string? RejectionReason { get; set; } // Lý do từ chối (nếu Rejected)

        public DateTime RequestDate { get; set; } = DateTime.Now;

        public DateTime? ProcessedDate { get; set; } // Ngày Admin xử lý

        public int? ProcessedByUserId { get; set; } // Admin nào duyệt/từ chối

        public int? CreatedStudentId { get; set; } // StudentId được tạo (nếu Approved)
    }
}