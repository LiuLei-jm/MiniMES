using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniMES.Infrastructure.Authentication;
using MiniMES.Infrastructure.Persistence;
using MiniMES.Infrastructure.Persistence.Seed;
using Xunit;

namespace MiniMES.ArchitectureTests;

public sealed class DataSeederTests
{
    [Fact]
    public async Task Seed_creates_an_idempotent_admin_with_the_admin_role_and_all_permissions()
    {
        var connectionString =
            $"Data Source=file:minimes-seed-{Guid.NewGuid():N}?mode=memory&cache=shared";
        await using var connection = new SqliteConnection(connectionString);
        await connection.OpenAsync();

        var dbOptions = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connectionString)
            .Options;
        await using var context = new AppDbContext(dbOptions);
        await context.Database.EnsureCreatedAsync();

        var passwordHasher = new PasswordHasher();
        var seedOptions = new AdminSeedOptions
        {
            Enabled = true,
            UserName = "admin"
        };

        var firstResult = await DataSeeder.SeedAsync(
            context,
            passwordHasher,
            seedOptions,
            allowGeneratedAdminPassword: true);
        var secondResult = await DataSeeder.SeedAsync(
            context,
            passwordHasher,
            seedOptions,
            allowGeneratedAdminPassword: true);

        Assert.NotNull(firstResult);
        Assert.True(firstResult.Created);
        Assert.NotNull(firstResult.GeneratedPassword);
        Assert.False(secondResult!.Created);
        Assert.Null(secondResult.GeneratedPassword);

        var admin = await context.Users.SingleAsync(x => x.UserName == "admin");
        Assert.True(passwordHasher.Verify(firstResult.GeneratedPassword, admin.PasswordHash));

        var adminRole = await context.Roles.SingleAsync(x => x.Name == "Admin");
        Assert.True(await context.UserRoles.AnyAsync(
            x => x.UserId == admin.Id && x.RoleId == adminRole.Id));
        Assert.Equal(
            await context.Permissions.CountAsync(),
            await context.RolePermissions.CountAsync(x => x.RoleId == adminRole.Id));
        Assert.Equal(1, await context.Users.CountAsync(x => x.UserName == "admin"));

        var permissionInfo = await new UserPermissionService(context)
            .GetPermissionsAsync(admin.Id);
        Assert.Contains("Admin", permissionInfo.Roles);
        Assert.Equal(
            await context.Permissions.CountAsync(),
            permissionInfo.Permissions.Count);
    }
}
