using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProjectManagement.Data.Identity;
using ProjectManagement.Data.Issues;
using ProjectManagement.Data.Projects;

namespace ProjectManagement;
public class AppDbContext : IdentityUserContext<User, Guid>
{
    public DbSet<Project> Projects { get; set; } = null!;

    public DbSet<Issue> Issues { get; set; }

    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        base.OnConfiguring(options);
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        var assemblyWithConfiguration = GetType().Assembly;
        builder.ApplyConfigurationsFromAssembly(assemblyWithConfiguration);
    }
}
