using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Phân công giảng viên dạy môn học
    /// </summary>
    public class InstructorAssignment
    {
        [Key]
        public int AssignmentId { get; set; }

        [Required]
        public int InstructorId { get; set; }

        [ForeignKey("InstructorId")]
        public virtual Instructor Instructor { get; set; } = null!;

        [Required]
        public int CourseId { get; set; }

        [ForeignKey("CourseId")]
        public virtual Course Course { get; set; } = null!;

        [Required]
        [Range(1, 9)]
        public int SemesterNumber { get; set; } // Kỳ được phân công (1-9)

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active"; // "Active", "Inactive"

        public DateTime AssignedDate { get; set; } = DateTime.Now;

        [Required]
        public int AssignedByUserId { get; set; } // Admin nào phân công

        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}