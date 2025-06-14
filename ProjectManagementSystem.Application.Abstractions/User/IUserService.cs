using ProjectManagementSystem.Application.Abstractions.User.Dtos;
using ProjectManagementSystem.Domain.Entities;
using System.Security.Claims;

namespace ProjectManagementSystem.Application.Abstractions.User
{
    public interface IUserService
    {
        Task<AppUser?> AddUser(UserDto userDto);
        Task<bool> DeleteUser(Guid id);
        Task<List<UserDto>> GetAllUsers();
        Task<List<UserDto>> GetAllUsersByTeamId(Guid id);
        Task<List<UserDto>> GetAllUsersExceptCurrent(ClaimsPrincipal principal, string teamId);
        Task<string> GetTeamId(ClaimsPrincipal principal);
        Task<AppUser> FindById(string userId);
        Task<Guid> GetCurrentUserId(ClaimsPrincipal principal);
    }
}
