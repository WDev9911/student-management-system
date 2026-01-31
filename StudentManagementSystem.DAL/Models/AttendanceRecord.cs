using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Tr?ng thái ?i?m danh c?a 1 sinh viên trong 1 bu?i h?c
    /// </summary>
    public class AttendanceRecord
    {
        [Key]
        public int AttendanceRecordId { get; set; }

        [Required]
        public int AttendanceSessionId { get; set; }

        [ForeignKey("AttendanceSessionId")]
        public virtual AttendanceSession AttendanceSession { get; set; } = null!;

        [Required]
        public int StudentId { get; set; }

        [ForeignKey("StudentId")]
        public virtual Student Student { get; set; } = null!;

        [Required]
        [StringLength(10)]
        public string Status { get; set; } = "Present"; // Present | Absent

        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}