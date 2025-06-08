using System.ComponentModel.DataAnnotations;

public class ChangePasswordViewModel
{
    [Required(ErrorMessage = "Email is Required")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Old Password is Required")]
    public string OldPassword { get; set; }

    [Required(ErrorMessage = "Password is Required")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required(ErrorMessage = "Confirm Password is Required")]
    [Compare("Password", ErrorMessage = "Passwords do not match")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; }
}