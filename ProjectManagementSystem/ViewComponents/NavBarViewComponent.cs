using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Models;

namespace ProjectManagementSystem.ViewComponents
{
    public class NavBarViewComponent : ViewComponent
    {
        private readonly List<NavBarItem> _navItems;
        private readonly ITeamService _teamService;
        private readonly IMemoryCache _cache;

        public NavBarViewComponent(IOptions<List<NavBarItem>> options,
            ITeamService teamService,
            IMemoryCache cache)
        {
            _navItems = options.Value;
            _teamService = teamService;
            _cache = cache;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var controller = RouteData.Values["controller"]?.ToString();
            var action = RouteData.Values["action"]?.ToString();
            var currentPath = $"{controller}/{action}";
            
            var visibleItems = _navItems
                .Where(item => item.VisibleFor == null ||
                               item.VisibleFor.Contains(currentPath, StringComparer.OrdinalIgnoreCase))
                .ToList();

            var team = await _teamService.GetTeamByUser(UserClaimsPrincipal);

            string title = string.Empty;

            if (UseDepartmentTitle(currentPath))
                title = team.TeamName ?? string.Empty;
            else
            {
                string userName = User.Identity.Name;

                if (!string.IsNullOrEmpty(userName) && _cache.TryGetValue($"{userName}_MainTitle", out string mainTitle))
                    title = mainTitle;
            }
            
            if (string.IsNullOrEmpty(title))
                title = team.TeamName ?? string.Empty;

            var model = new Navbar { MainTitle = title, NavbarItems = visibleItems };

            return View(model);
        }

        private static bool UseDepartmentTitle(string path)
        {
            var depTitleUsageList = new List<string>
            {
                "Dept/DeptMain", 
                "Dept/DeptCrew", 
                "Project/GetMyTasks",
                "Board/Index"
            };

            return depTitleUsageList.Contains(path);
        }
    }
}
