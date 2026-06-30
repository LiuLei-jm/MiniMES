using MiniMES.Application.Common.Interfaces;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Persistence.Interceptors;

namespace MiniMES.Infrastructure.Persistence;

public class AppDbContext : DbContext, IApplicationDbContext
{
    private readonly AppDbCommandInterceptor _interceptor;
    public AppDbContext(DbContextOptions<AppDbContext> options, AppDbCommandInterceptor interceptor)
        : base(options)
    {
        _interceptor = interceptor;
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<Permission> Permissions => Set<Permission>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<UserRole>()
            .HasKey(x => new { x.UserId, x.RoleId });
        modelBuilder.Entity<RolePermission>()
            .HasKey(x => new { x.RoleId, x.PermissionId });
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(_interceptor);
    }
}
