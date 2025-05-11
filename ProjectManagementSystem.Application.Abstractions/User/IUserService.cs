using ProjectManagementSystem.Application.Abstractions.User.Dtos;
using ProjectManagementSystem.Domain.Entities;
using System.Security.Claims;

namespace ProjectManagementSystem.Application.Abstractions.User
{
    public interface IUserService
    {
        Task<bool> AddUser(UserDto userDto);
        Task<bool> DeleteUser(Guid id);
        Task<List<UserDto>> GetAllUsers();
        Task<List<UserDto>> GetAllUsersByTeamId(Guid id);
        Task<string> GetTeamId(ClaimsPrincipal principal);
        Task<AppUser> FindById(string userId);
    }
}
