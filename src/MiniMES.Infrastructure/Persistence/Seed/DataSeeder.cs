using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        if (!context.Permissions.Any())
        {
            var permissions = PermissionSeeder.Seed().ToList();
            context.Permissions.AddRange(permissions);

            var roles = RoleSeeder.Seed();
            context.Roles.AddRange(roles);

            await context.SaveChangesAsync();

            var rolePermissions = RolePermissionSeeder.Seed(roles, permissions);
            context.RolePermissions.AddRange(rolePermissions);
            await context.SaveChangesAsync();
        }
    }
}
