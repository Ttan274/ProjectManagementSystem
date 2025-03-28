using Microsoft.AspNetCore.Identity;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Application.Abstractions.User.Dtos;
using ProjectManagementSystem.Domain.Entities;

namespace ProjectManagementSystem.Application.User
{
    public class LoginService : ILoginService
    {
        private readonly SignInManager<AppUser> _signInManager;
        private readonly UserManager<AppUser> _userManager;

        public LoginService(SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<bool> CheckRole(string email, string role)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
                return false;

            try
            {
                var user = await _userManager.FindByEmailAsync(email);

                if (user == null)
                    throw new Exception();

                var response = await _userManager.IsInRoleAsync(user, role);

                return response;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> Login(UserDto userDto)
        {
            if (userDto is null)
                return false;

            try
            {
                var user = await _userManager.FindByEmailAsync(userDto.Email);

                if (user == null)
                    throw new Exception();

                var response = await _signInManager.PasswordSignInAsync(user, userDto.Password, false, false);

                return response.Succeeded;  
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
