using System.ComponentModel.DataAnnotations;

namespace StudentManagementSystem.Web.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username là b?t bu?c")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password là b?t bu?c")]
        [StringLength(100)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public bool RememberMe { get; set; }
    }
}