using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjectManagementSystem.Application.Abstractions.AppInfo;
using ProjectManagementSystem.Application.Abstractions.Chat;
using ProjectManagementSystem.Application.Abstractions.Documentation;
using ProjectManagementSystem.Application.Abstractions.GitHubRepoAnalytics;
using ProjectManagementSystem.Application.Abstractions.Project;
using ProjectManagementSystem.Application.Abstractions.Repositories.AppInfo;
using ProjectManagementSystem.Application.Abstractions.Repositories.Chat;
using ProjectManagementSystem.Application.Abstractions.Repositories.Documentation;
using ProjectManagementSystem.Application.Abstractions.Repositories.Project;
using ProjectManagementSystem.Application.Abstractions.Repositories.Sprint;
using ProjectManagementSystem.Application.Abstractions.Repositories.Task;
using ProjectManagementSystem.Application.Abstractions.Repositories.Team;
using ProjectManagementSystem.Application.Abstractions.Sprint;
using ProjectManagementSystem.Application.Abstractions.SubTaskProducer;
using ProjectManagementSystem.Application.Abstractions.Task;
using ProjectManagementSystem.Application.Abstractions.Team;
using ProjectManagementSystem.Application.Abstractions.User;
using ProjectManagementSystem.Application.AppInfo;
using ProjectManagementSystem.Application.Chat;
using ProjectManagementSystem.Application.Documentation;
using ProjectManagementSystem.Application.GitHubRepoAnalytics;
using ProjectManagementSystem.Application.Project;
using ProjectManagementSystem.Application.Sprint;
using ProjectManagementSystem.Application.SubTaskProducer;
using ProjectManagementSystem.Application.Task;
using ProjectManagementSystem.Application.Team;
using ProjectManagementSystem.Application.User;
using ProjectManagementSystem.Domain.Entities;
using ProjectManagementSystem.Persistance.DbContext;
using ProjectManagementSystem.Persistance.Repositories.AppInfo;
using ProjectManagementSystem.Persistance.Repositories.Chat;
using ProjectManagementSystem.Persistance.Repositories.Documentation;
using ProjectManagementSystem.Persistance.Repositories.Project;
using ProjectManagementSystem.Persistance.Repositories.Sprint;
using ProjectManagementSystem.Persistance.Repositories.Task;
using ProjectManagementSystem.Persistance.Repositories.Team;

namespace ProjectManagementSystem.Persistance
{
    public static class ServiceRegistration
    {
        public static void AddRequiredServices(this IServiceCollection services, ConfigurationManager config)
        {
            //Json connection to the database
            var mySql = config.GetConnectionString("MySql") ?? string.Empty;

            //Database
            services.AddDbContext<AppDbContext>(options =>
            options.UseMySQL(mySql)); //opt => opt.MigrationsAssembly("ProjectManagementSystem"))


            //Identity
            services.AddIdentity<AppUser, AppRole>(options => //braya göre bi validasyon gösterelim ekranda
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 4;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 0;
            })
            .AddEntityFrameworkStores<AppDbContext>()
            .AddDefaultTokenProviders();

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

            //Sprint
            services.AddScoped<ISprintReadRepository, SprintReadRepository>();
            services.AddScoped<ISprintWriteRepository, SprintWriteRepository>();
            services.AddScoped<ISprintService, SprintService>();

            //Task
            services.AddScoped<ITaskReadRepository, TaskReadRepository>();
            services.AddScoped<ITaskWriteRepository, TaskWriteRepository>();
            services.AddScoped<ITaskService, TaskService>();

            //SubTask
            services.AddScoped<ISubTaskProducerService, SubTaskProducerService>();

            //Documentation
            services.AddScoped<IDocumentationReadRepository, DocumentationReadRepository>();
            services.AddScoped<IDocumentationWriteRepository, DocumentationWriteRepository>();
            services.AddScoped<IDocumentationService, DocumentationService>();

            //Task
            services.AddScoped<IAppInfoService, AppInfoService>();
            services.AddScoped<IAppInfoReadRepository, AppInfoReadRepository>();
            services.AddScoped<IAppInfoWriteRepository, AppInfoWriteRepository>();

            //Chat Messages
            services.AddScoped<IChatReadRepository, ChatReadRepository>();
            services.AddScoped<IChatWriteRepository, ChatWriteRepository>();
            services.AddScoped<IChatService, ChatService>();

            //GithubAnalytics
            services.AddScoped<IGitHubRepoAnalyticsService, GitHubRepoAnalyticsService>();

            services.AddMemoryCache();

        }
    }
}
