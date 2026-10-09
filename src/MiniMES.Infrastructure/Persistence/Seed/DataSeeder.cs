using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Domain.Entities;
using System.Security.Cryptography;

namespace MiniMES.Infrastructure.Persistence.Seed;

public static class DataSeeder
{
    public static async Task<AdminSeedResult?> SeedAsync(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        AdminSeedOptions? adminOptions = null,
        bool allowGeneratedAdminPassword = false,
        CancellationToken cancellationToken = default)
    {
        await SeedAuthorizationAsync(context, cancellationToken);

        if (adminOptions is not { Enabled: true })
        {
            return null;
        }

        return await SeedAdminAsync(
            context,
            passwordHasher,
            adminOptions,
            allowGeneratedAdminPassword,
            cancellationToken);
    }

    private static async Task SeedAuthorizationAsync(
        AppDbContext context,
        CancellationToken cancellationToken)
    {
        var permissionsByCode = await context.Permissions
            .ToDictionaryAsync(x => x.Code, cancellationToken);

        foreach (var permission in PermissionSeeder.Seed())
        {
            if (!permissionsByCode.ContainsKey(permission.Code))
            {
                context.Permissions.Add(permission);
                permissionsByCode.Add(permission.Code, permission);
            }
        }

        var rolesByName = await context.Roles
            .ToDictionaryAsync(x => x.Name, cancellationToken);

        foreach (var role in RoleSeeder.Seed())
        {
            if (!rolesByName.ContainsKey(role.Name))
            {
                context.Roles.Add(role);
                rolesByName.Add(role.Name, role);
            }
        }

        await context.SaveChangesAsync(cancellationToken);

        var adminRole = rolesByName["Admin"];
        var assignedPermissionIds = await context.RolePermissions
            .Where(x => x.RoleId == adminRole.Id)
            .Select(x => x.PermissionId)
            .ToListAsync(cancellationToken);
        var assignedPermissionIdSet = assignedPermissionIds.ToHashSet();

        foreach (var permission in permissionsByCode.Values)
        {
            if (!assignedPermissionIdSet.Contains(permission.Id))
            {
                context.RolePermissions.Add(new RolePermission
                {
                    RoleId = adminRole.Id,
                    PermissionId = permission.Id
                });
            }
        }

        await context.SaveChangesAsync(cancellationToken);
    }

    private static async Task<AdminSeedResult> SeedAdminAsync(
        AppDbContext context,
        IPasswordHasher passwordHasher,
        AdminSeedOptions options,
        bool allowGeneratedPassword,
        CancellationToken cancellationToken)
    {
        var userName = options.UserName.Trim();
        if (string.IsNullOrWhiteSpace(userName))
        {
            throw new InvalidOperationException("DataSeed:Admin:UserName is required.");
        }

        var adminRole = await context.Roles
            .SingleAsync(x => x.Name == "Admin", cancellationToken);
        var adminUser = await context.Users
            .SingleOrDefaultAsync(x => x.UserName == userName, cancellationToken);
        string? generatedPassword = null;
        var created = false;

        if (adminUser is null)
        {
            var password = options.Password;
            if (string.IsNullOrWhiteSpace(password))
            {
                if (!allowGeneratedPassword)
                {
                    throw new InvalidOperationException(
                        "DataSeed:Admin:Password is required when administrator seeding is enabled outside Development or Testing.");
                }

                generatedPassword = GeneratePassword();
                password = generatedPassword;
            }

            if (password.Length < 6)
            {
                throw new InvalidOperationException("DataSeed:Admin:Password must be at least 6 characters.");
            }

            adminUser = new User(userName, passwordHasher.Hash(password));
            context.Users.Add(adminUser);
            await context.SaveChangesAsync(cancellationToken);
            created = true;
        }

        if (!await context.UserRoles.AnyAsync(
                x => x.UserId == adminUser.Id && x.RoleId == adminRole.Id,
                cancellationToken))
        {
            context.UserRoles.Add(new UserRole
            {
                UserId = adminUser.Id,
                RoleId = adminRole.Id
            });
            await context.SaveChangesAsync(cancellationToken);
        }

        return new AdminSeedResult(userName, created, generatedPassword);
    }

    private static string GeneratePassword() =>
        $"Adm1!{Convert.ToHexString(RandomNumberGenerator.GetBytes(12))}";
}
