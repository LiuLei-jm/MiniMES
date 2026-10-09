using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Features.Users.DeleteUser;
using MiniMES.Application.Features.Users.GetProfile;
using MiniMES.Application.Features.Users.GetUsers;
using MiniMES.Application.Features.Users.UpdateUser;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Persistence;
using Xunit;

namespace MiniMES.ArchitectureTests;

public sealed class UserManagementTests
{
    [Fact]
    public async Task User_queries_updates_and_self_protection_work_together()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();

        var admin = new User("admin", "hash");
        var target = new User("operator", "hash");
        var role = new Role("Admin");
        var permission = new Permission("users:view", "查看用户");
        db.AddRange(admin, target, role, permission);
        db.UserRoles.Add(new UserRole { UserId = admin.Id, RoleId = role.Id });
        db.RolePermissions.Add(new RolePermission { RoleId = role.Id, PermissionId = permission.Id });
        await db.SaveChangesAsync();

        var page = await new GetUsersQueryHandler(db).Handle(new GetUsersQuery("oper", 1, 20), default);
        Assert.True(page.IsSuccess);
        Assert.Single(page.Value!.Items);
        Assert.Equal("operator", page.Value.Items[0].UserName);

        var update = await new UpdateUserCommandHandler(db, new TestCurrentUser(admin.Id))
            .Handle(new UpdateUserCommand(target.Id, "operator-2", false), default);
        Assert.True(update.IsSuccess);
        Assert.Equal("operator-2", target.UserName);
        Assert.False(target.IsActive);

        var selfDelete = await new DeleteUserCommandHandler(db, new TestCurrentUser(admin.Id))
            .Handle(new DeleteUserCommand(admin.Id), default);
        Assert.True(selfDelete.IsFailure);
        Assert.Equal(UserErrors.CannotDisableSelf.Code, selfDelete.Error.Code);

        var profile = await new GetProfileQueryHandler(db, new TestCurrentUser(admin.Id))
            .Handle(new GetProfileQuery(), default);
        Assert.True(profile.IsSuccess);
        Assert.Contains("Admin", profile.Value!.Roles);
        Assert.Contains("users:view", profile.Value.Permissions);
    }

    private sealed class TestCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid UserId { get; } = userId;
        public string UserName => "admin";
        public List<string> Roles => ["Admin"];
        public bool IsAuthenticated => true;
    }
}
