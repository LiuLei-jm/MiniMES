using MiniMES.Domain.Entities;

namespace MiniMES.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    public DbSet<User> Users { get; }
    public DbSet<Role> Roles { get; }
    public DbSet<Permission> Permissions { get; }
    public DbSet<UserRole> UserRoles { get; }
    public DbSet<RolePermission> RolePermissions { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
