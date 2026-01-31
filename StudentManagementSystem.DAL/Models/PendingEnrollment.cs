using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Danh sách môn h?c ch? ??ng ký (cho môn r?t ho?c thi?u prerequisite)
    /// </summary>
    public class PendingEnrollment
    {
        [Key]
        public int PendingEnrollmentId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;

        [Required]
        public int SemesterNumber { get; set; } // K? d? ??nh ??ng ký

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending"; // "Pending", "Registered", "Cancelled"

        [Required]
        public bool IsRetake { get; set; } = false; // true = Môn h?c l?i

        [Column(TypeName = "decimal(18,2)")]
        public decimal RequiredFee { get; set; } // H?c phí c?n tr?

        [StringLength(500)]
        public string? Reason { get; set; } // Lý do

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime? RegisteredDate { get; set; }
    }
}