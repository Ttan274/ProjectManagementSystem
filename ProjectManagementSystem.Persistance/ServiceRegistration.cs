using ProjectManagementSystem.Persistance.DbContext;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Application.User;

namespace ProjectManagementSystem.Persistance
{
    public static class ServiceRegistration
    {
        public static void AddRequiredServices(this IServiceCollection services, ConfigurationManager config)
        {
            var mySql = config.GetConnectionString("MySql") ?? string.Empty;

            services.AddDbContext<AppDbContext>(options => options.UseMySQL(mySql));

            services.AddIdentity<AppUser, AppRole>().AddEntityFrameworkStores<AppDbContext>();

            services.AddAutoMapper(typeof(Application.Mappings.Profiles));

            //Adding Custom Services

            //User
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILoginService, LoginService>();
        }
    }
}
