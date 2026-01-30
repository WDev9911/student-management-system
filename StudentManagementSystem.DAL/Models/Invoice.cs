using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Hóa đơn thanh toán
    /// </summary>
    public class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string InvoiceNumber { get; set; } = string.Empty; // "INV20250129001"

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999999)]
        public decimal TotalAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = "Enrollment"; // "Enrollment", "Tuition", "Other"

        [StringLength(1000)]
        public string? Description { get; set; } // Chi tiết: "Đăng ký 4 môn Kỳ 1: IT101, IT102..."

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Paid"; // "Paid", "Unpaid", "Refunded", "Cancelled"

        public DateTime? PaidDate { get; set; }

        public int? TransactionId { get; set; } // Link đến Transaction

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}