using ProjectManagementSystem.Persistance.DbContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Application.User;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Team;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Project;

namespace ProjectManagementSystem.Persistance
{
    public static class ServiceRegistration
    {
        public static void AddRequiredServices(this IServiceCollection services, ConfigurationManager config)
        {
            //Json connection to the database
            var mySql = config.GetConnectionString("MySql") ?? string.Empty;

            //Database
            services.AddDbContext<AppDbContext>(options => options.UseMySQL(mySql));

            //Identity
            services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<AppDbContext>();

            //Mappings
            services.AddAutoMapper(typeof(Application.Mappings.Profiles));

            //Adding Custom Services
            //User
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILoginService, LoginService>();

            //Team
            services.AddScoped<ITeamService, TeamService>();

            //Project
            services.AddScoped<IProjectService, ProjectService>();
        }
    }
}
