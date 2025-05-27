using System.ComponentModel.DataAnnotations;

namespace ProjectManagementSystem.Models
{
    public class ResetPasswordRequestModel
    {
        [Required(ErrorMessage = "Email is Required")]
        public string Email { get; set; }
    }
}
