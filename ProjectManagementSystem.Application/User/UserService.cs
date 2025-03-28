using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Application.Abstractions.User.Dtos;
using ProjectManagementSystem.Domain.Entities;
using System.Security.Claims;

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

        public async Task<bool> AddUser(UserDto userDto)
        {
            if (userDto is null)
                return false;

            try
            {
                userDto.UserName = userDto.Name + userDto.Surname;

                var mappedResult = _mapper.Map<UserDto, AppUser>(userDto);

                var response = await _userManager.CreateAsync(mappedResult, userDto.Password);

                var result = await _userManager.AddToRoleAsync(mappedResult, "Employee");

                return response.Succeeded && result.Succeeded;
            }
            catch (Exception)
            {
                return false;
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

        public async Task<UserDto> GetAdmin(ClaimsPrincipal principal)
        {
            try
            {
                var user = await _userManager.GetUserAsync(principal);

                if (user is null)
                    throw new Exception();

                var mappedResult = _mapper.Map<AppUser, UserDto>(user);

                return mappedResult;
            }
            catch (Exception)
            {
                return new UserDto();
            }
        }
    }
}
