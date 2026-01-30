using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Thông báo cho người dùng
    /// </summary>
    public class Notification
    {
        [Key]
        public int NotificationId { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty; // "Đăng ký môn thành công"

        [Required]
        [StringLength(1000)]
        public string Message { get; set; } = string.Empty; // "Bạn đã đăng ký 4 môn Kỳ 1 thành công!"

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = "Info"; // "Info", "Success", "Warning", "Danger"

        [StringLength(50)]
        public string? Icon { get; set; } // "bell", "check-circle", "exclamation-triangle"

        public bool IsRead { get; set; } = false;

        [StringLength(500)]
        public string? ActionUrl { get; set; } // "/Student/Enrollment/Details/123"

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? ReadDate { get; set; }
    }
}