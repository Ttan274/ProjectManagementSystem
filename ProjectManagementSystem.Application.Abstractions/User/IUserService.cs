using ProjectManagementSystem.Application.Abstractions.User.Dtos;
using System.Security.Claims;

namespace ProjectManagementSystem.Application.Abstractions.User
{
    public interface IUserService
    {
        Task<bool> AddUser(UserDto userDto);
        Task<bool> DeleteUser(Guid id);
        Task<List<UserDto>> GetAllUsers();
        Task<UserDto> GetAdmin(ClaimsPrincipal principal);
    }
}
