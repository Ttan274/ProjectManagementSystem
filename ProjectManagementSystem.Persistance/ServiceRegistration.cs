using ProjectManagementSystem.Persistance.DbContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Application.User;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Team;
using ProjectManagementSystem.Application.Abstractions.Repositories.Team;
using ProjectManagementSystem.Persistance.Repositories.Team;
using ProjectManagementSystem.Application.Abstractions.Repositories.Project;
using ProjectManagementSystem.Persistance.Repositories.Project;
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
            services.AddScoped<ITeamReadRepository, TeamReadRepository>();
            services.AddScoped<ITeamWriteRepository, TeamWriteRepository>();
            services.AddScoped<ITeamService, TeamService>();

            //Project
            services.AddScoped<IProjectReadRepository, ProjectReadRepository>();
            services.AddScoped<IProjectWriteRepository, ProjectWriteRepository>();
            services.AddScoped<IProjectService, ProjectService>();
        }
    }
}
