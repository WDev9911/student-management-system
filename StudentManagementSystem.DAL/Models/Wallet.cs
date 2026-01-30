using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Ví tiền của sinh viên (mỗi sinh viên có 1 ví)
    /// </summary>
    public class Wallet
    {
        [Key]
        public int WalletId { get; set; }

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999999, ErrorMessage = "Số dư không hợp lệ")]
        public decimal Balance { get; set; } = 0; // Số dư hiện tại

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    }
}