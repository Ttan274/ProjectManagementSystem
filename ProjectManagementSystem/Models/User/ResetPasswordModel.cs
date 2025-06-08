using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models
{
    public class ResetPasswordModel
    {
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
        public string Token { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm Password is Required")]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
