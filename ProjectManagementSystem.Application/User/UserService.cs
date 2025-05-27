using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Application.Abstractions.User.Dtos;
using ProjectManagementSystem.Domain.Entities;

namespace ProjectManagementSystem.Application.User
{
    public class UserService : IUserService
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<AppUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<AppUser?> AddUser(UserDto userDto)
        {
            try
            {
                if (userDto is null)
                    throw new Exception();

                userDto.UserName = userDto.Name + userDto.Surname;

                var mappedResult = _mapper.Map<UserDto, AppUser>(userDto);

                mappedResult.MustChangePassword = true;

                var response = await _userManager.CreateAsync(mappedResult, userDto.Password);

                var result = await _userManager.AddToRoleAsync(mappedResult, "Employee");

                if (!(response.Succeeded && result.Succeeded))
                    throw new Exception();

                return mappedResult;
            }
            catch (Exception)
            {
               return null;
            }
        }

        public async Task<bool> DeleteUser(Guid id)
        {
            if (id == Guid.Empty)
                return false;

            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());

                if (user == null)
                    return false;

                var response = await _userManager.DeleteAsync(user);

                return response.Succeeded;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<UserDto>> GetAllUsers()
        {
            try
            {
                var users = await _userManager.Users.ToListAsync();

                if (users is null)
                    return [];

                var mappedResult = _mapper.Map<List<AppUser>, List<UserDto>>(users);

                return mappedResult;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<List<UserDto>> GetAllUsersByTeamId(Guid id)
        {
            try
            {
                var users = await _userManager.Users.Where(x => x.TeamId == id).ToListAsync();

                if (users is null)
                    return [];

                var mappedResult = _mapper.Map<List<AppUser>, List<UserDto>>(users);

                return mappedResult;
            }
            catch (Exception)
            {
                return [];
            }
        }

        public async Task<string> GetTeamId(ClaimsPrincipal principal)
        {
            try
            {
                var user = await _userManager.GetUserAsync(principal);

                if (user is null)
                    throw new Exception();

                var mappedResult = _mapper.Map<AppUser, UserDto>(user);

                return mappedResult.TeamId;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public async Task<AppUser> FindById(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user is null)
                    throw new Exception();

                return user;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
