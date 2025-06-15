using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagementSystem.Domain.Entities;

namespace ProjectManagementSystem.Persistance.DbContext
{
    public class AppDbContext : IdentityDbContext<AppUser, AppRole, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Team> Teams { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<Sprint> Sprints { get; set; }
        public DbSet<Domain.Entities.Task> Tasks { get; set; }
        public DbSet<Documentation> Documentations { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<AppInfo> AppInfos { get; set; }
        public DbSet<ProjectTeamConfig> ProjectTeamConfigs { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>()
                   .HasMany(x => x.Tasks)
                   .WithOne(x => x.AppUser)
                   .HasForeignKey(x => x.UserId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Domain.Entities.Task>()
                   .HasOne(x => x.Documentation)
                   .WithOne(x => x.Task)
                   .HasForeignKey<Documentation>(x => x.TaskId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Domain.Entities.Task>()
                .HasMany(t => t.DependentTasks)
                .WithOne(t => t.ParentTask)
                .HasForeignKey(t => t.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Project>()
                .HasMany(x => x.Applications)
                .WithOne(x => x.Project)
                .HasForeignKey(x => x.ProjectId)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
