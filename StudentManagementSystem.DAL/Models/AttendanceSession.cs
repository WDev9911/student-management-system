using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Bu?i ?i?m danh c?a m?t l?p vào m?t ngày
    /// </summary>
    public class AttendanceSession
    {
        [Key]
        public int AttendanceSessionId { get; set; }

        [Required]
        public int ClassId { get; set; }

        [ForeignKey("ClassId")]
        public virtual CourseClass CourseClass { get; set; } = null!;

        [Required]
        public int InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public virtual Instructor Instructor { get; set; } = null!;

        [Required]
        public DateTime SessionDate { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<AttendanceRecord> Records { get; set; } = new List<AttendanceRecord>();
    }
}