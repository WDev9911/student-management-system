using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Sinh viên
    /// </summary>
    public class Student
    {
        [Key]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Mã sinh viên là bắt buộc")]
        [StringLength(20)]
        public string StudentCode { get; set; } = string.Empty; // SV001, SV002

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
        public string? Gender { get; set; } // Male, Female, Other

        [StringLength(500)]
        public string? Address { get; set; }

        [Required]
        public int MajorId { get; set; }

        [ForeignKey("MajorId")]
        public virtual Major Major { get; set; } = null!;

        [Required]
        [Range(1, 9)]
        public int CurrentSemester { get; set; } = 1;

        [Column(TypeName = "decimal(4,2)")]
        [Range(0, 4, ErrorMessage = "GPA từ 0-4")]
        public decimal CGPA { get; set; } = 0; // Cumulative GPA (4.0 scale)

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // Active, Graduated, Suspended, Dropped

        public DateTime EnrollmentDate { get; set; } = DateTime.Now; // Ngày nhập học

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual Wallet? Wallet { get; set; }
        public virtual User? User { get; set; }
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public virtual ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}