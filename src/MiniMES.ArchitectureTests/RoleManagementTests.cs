using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Features.Roles.AssignUserRoles;
using MiniMES.Application.Features.Roles.CreateRole;
using MiniMES.Application.Features.Roles.DeleteRole;
using MiniMES.Application.Features.Roles.GetRoles;
using MiniMES.Application.Features.Roles.UpdateRole;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Persistence;
using Xunit;

namespace MiniMES.ArchitectureTests;

public sealed class RoleManagementTests
{
    [Fact]
    public async Task Roles_can_be_managed_and_assigned_with_admin_guards()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();
        var current = new User("admin", "hash");
        var target = new User("operator", "hash");
        var adminRole = new Role("Admin");
        db.AddRange(current, target, adminRole);
        await db.SaveChangesAsync();

        var created = await new CreateRoleCommandHandler(db).Handle(new CreateRoleCommand("Supervisor"), default);
        Assert.True(created.IsSuccess);
        var roleId = created.Value;
        Assert.NotEqual(Guid.Empty, roleId);

        var assigned = await new AssignUserRolesCommandHandler(db, new TestCurrentUser(current.Id))
            .Handle(new AssignUserRolesCommand(target.Id, [roleId]), default);
        Assert.True(assigned.IsSuccess);
        Assert.True(await db.UserRoles.AnyAsync(x => x.UserId == target.Id && x.RoleId == roleId));

        var list = await new GetRolesQueryHandler(db).Handle(new GetRolesQuery(), default);
        Assert.Equal(1, list.Value!.Single(x => x.Id == roleId).UserCount);

        var updated = await new UpdateRoleCommandHandler(db).Handle(new UpdateRoleCommand(roleId, "Leader"), default);
        Assert.True(updated.IsSuccess);

        var protectedRole = await new DeleteRoleCommandHandler(db).Handle(new DeleteRoleCommand(adminRole.Id), default);
        Assert.Equal(RoleErrors.BuiltInRole.Code, protectedRole.Error.Code);

        var deleted = await new DeleteRoleCommandHandler(db).Handle(new DeleteRoleCommand(roleId), default);
        Assert.True(deleted.IsSuccess);
        Assert.False(await db.Roles.AnyAsync(x => x.Id == roleId));
        Assert.False(await db.UserRoles.AnyAsync(x => x.RoleId == roleId));
    }

    private sealed class TestCurrentUser(Guid userId) : ICurrentUser
    {
        public Guid UserId { get; } = userId;
        public string UserName => "admin";
        public List<string> Roles => ["Admin"];
        public bool IsAuthenticated => true;
    }
}
