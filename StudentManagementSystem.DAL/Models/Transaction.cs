using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Giao dịch (Nạp tiền, Trừ tiền, Hoàn tiền)
    /// </summary>
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [Required]
        public int WalletId { get; set; }

        [ForeignKey("WalletId")]
        public virtual Wallet Wallet { get; set; } = null!;

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        [Required]
        [StringLength(50)]
        public string TransactionCode { get; set; } = string.Empty; // "TXN20250129001"

        [Required]
        [StringLength(20)]
        public string Type { get; set; } = string.Empty; // "Deposit", "Deduct", "Refund"

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999999)]
        public decimal Amount { get; set; } // Số tiền giao dịch

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceBefore { get; set; } // Số dư trước giao dịch

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal BalanceAfter { get; set; } // Số dư sau giao dịch

        [StringLength(500)]
        public string? Description { get; set; } // "Nạp tiền MoMo", "Đăng ký IT101-A"

        [StringLength(20)]
        public string Status { get; set; } = "Success"; // "Pending", "Success", "Failed"

        [StringLength(50)]
        public string? PaymentMethod { get; set; } // "MoMo", "BankTransfer", "Cash"

        public int? EnrollmentId { get; set; } // Link đến Enrollment nếu là trừ tiền đăng ký môn

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}