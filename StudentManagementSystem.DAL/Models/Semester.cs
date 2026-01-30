using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Học kỳ (9 kỳ trong 3 năm)
    /// </summary>
    public class Semester
    {
        [Key]
        public int SemesterId { get; set; }

        [Required]
        [Range(1, 9, ErrorMessage = "Số kỳ từ 1-9")]
        public int SemesterNumber { get; set; } // 1-9

        [Required]
        [StringLength(50)]
        public string SemesterName { get; set; } = string.Empty; // "Kỳ 1 - 2024-2025"

        [Required]
        [StringLength(20)]
        public string AcademicYear { get; set; } = string.Empty; // "2024-2025"

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Upcoming"; // Upcoming, Active, Completed

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<CourseClass> CourseClasses { get; set; } = new List<CourseClass>();
        public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}