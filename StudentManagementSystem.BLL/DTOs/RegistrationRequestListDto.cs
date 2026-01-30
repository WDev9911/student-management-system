namespace StudentManagementSystem.BLL.DTOs
{
    public class RegistrationRequestListDto
    {
        public int RequestId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public DateTime RequestDate { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}