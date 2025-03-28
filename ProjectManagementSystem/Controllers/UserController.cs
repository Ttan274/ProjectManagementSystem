using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Persistance.DbContext;
using ProjectManagementSystem.ViewModel;

namespace ProjectManagementSystem.Controllers
{
    public class UserController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly AppDbContext _context;

        public UserController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult CreateUser()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUserViewModel userVM)
        {
            var user = await _userManager.FindByEmailAsync(userVM.Email);
            if(user != null)
            {
                TempData["Error"] = "Email has been taken";
                return View(userVM);
            }

            if(ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = "TestObject",
                    Email = userVM.Email,
                    Gender = Common.Enums.Gender.Male,
                    TeamId = Guid.Parse("BB212DDB-0C15-4D63-8080-92BE98B309FD")
                };

                IdentityResult result = await _userManager.CreateAsync(appUser, userVM.Password);

                if (result.Succeeded)
                {
                    _context.Update(appUser);
                    _context.SaveChanges();
                    return RedirectToAction("Login", "User");
                }
                  
            }

            return View();
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            if (!ModelState.IsValid) return View(login);

            var user = await _userManager.FindByEmailAsync(login.Email);

            if(user != null)
            {
                var result = await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
                if(result.Succeeded)
                    return RedirectToAction("Index", "Home");
            }

            return View();
        }
    }
}
