using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.BLL.DTOs
{
    public class CreateInstructorDto
    {
        [Required(ErrorMessage = "Full Name is required")]
        [StringLength(100)]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone")]
        [StringLength(20)]
        public string? Phone { get; set; }

        [StringLength(100)]
        public string? Department { get; set; }

        [StringLength(200)]
        public string? Specialization { get; set; }

        [StringLength(200)]
        public string? Qualifications { get; set; }
    }

    public class InstructorListDto
    {
        public int InstructorId { get; set; }
        public string InstructorCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class InstructorDetailDto
    {
        public int InstructorId { get; set; }
        public string InstructorCode { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Phone { get; set; }
        public string? Department { get; set; }
        public string? Specialization { get; set; }
        public string? Qualifications { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public string? Username { get; set; }
    }
}