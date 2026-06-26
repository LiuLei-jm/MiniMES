using MiniMES.Application.Common.Security;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(AppDbContext context, IPasswordHasher passwordHasher)
    {
        if (!context.Roles.Any())
        {
            var hasher = passwordHasher.Hash("admin12345");
            var admin = new User("admin", hasher);
            var adminRole = new Role("Admin");
            context.Users.Add(admin);
            context.Roles.Add(adminRole);
            context.UserRoles.Add(new UserRole
            {
                UserId = admin.Id,
                RoleId = adminRole.Id
            });
            var perm = new Permission("user.Create", "Create user");
            context.Permissions.Add(perm);
            context.RolePermissions.Add(new RolePermission
            {
                RoleId = adminRole.Id,
                PermissionId = perm.Id
            });
            await context.SaveChangesAsync();
        }
    }
}
