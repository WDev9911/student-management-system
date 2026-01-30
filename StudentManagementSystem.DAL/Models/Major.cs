using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.DAL.Models
{
    /// <summary>
    /// Chuyên ngành đào tạo (IT, English, Business)
    /// </summary>
    public class Major
    {
        [Key]
        public int MajorId { get; set; }

        [Required(ErrorMessage = "Mã chuyên ngành là bắt buộc")]
        [StringLength(20)]
        public string MajorCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Tên chuyên ngành là bắt buộc")]
        [StringLength(200)]
        public string MajorName { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Description { get; set; }

        public int DurationYears { get; set; } = 3;

        public int TotalSemesters { get; set; } = 9;

        public int TotalCredits { get; set; } = 108;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Navigation properties
        public virtual ICollection<Student> Students { get; set; } = new List<Student>();
        public virtual ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}