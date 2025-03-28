using ProjectManagementSystem.Application.Abstractions.Base;

namespace ProjectManagementSystem.Application.Abstractions.User.Dtos
{
    public class UserDto : BaseDto
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }
        public string? UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
