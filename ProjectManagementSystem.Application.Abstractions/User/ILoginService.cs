using ProjectManagementSystem.Application.Abstractions.User.Dtos;

namespace ProjectManagementSystem.Application.Abstractions.User
{
    public interface ILoginService
    {
        Task<bool> Login(UserDto userDto);
        Task<bool> CheckRole(string email, string role);
    }
}
