namespace StudentManagementSystem.BLL.DTOs
{
    public class AuthUserDto
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? StudentId { get; set; }
        public int? InstructorId { get; set; }
    }
}