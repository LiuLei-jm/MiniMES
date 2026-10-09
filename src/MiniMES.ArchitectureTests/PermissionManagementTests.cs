using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using MiniMES.Application.Authentication.Constrants;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Features.Permissions.AssignRolePermissions;
using MiniMES.Application.Features.Permissions.CreatePermission;
using MiniMES.Application.Features.Permissions.DeletePermission;
using MiniMES.Application.Features.Permissions.GetPermissions;
using MiniMES.Application.Features.Permissions.UpdatePermission;
using MiniMES.Domain.Entities;
using MiniMES.Infrastructure.Persistence;
using Xunit;

namespace MiniMES.ArchitectureTests;

public sealed class PermissionManagementTests
{
    [Fact]
    public async Task Permissions_can_be_managed_and_assigned_dynamically()
    {
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();
        var options = new DbContextOptionsBuilder<AppDbContext>().UseSqlite(connection).Options;
        await using var db = new AppDbContext(options);
        await db.Database.EnsureCreatedAsync();
        var role = new Role("Supervisor");
        var adminPermission = new Permission(Permissions.System.Admin, "系统管理员权限");
        db.AddRange(role, adminPermission);
        await db.SaveChangesAsync();

        var created = await new CreatePermissionCommandHandler(db)
            .Handle(new CreatePermissionCommand("quality:view", "查看质量"), default);
        Assert.True(created.IsSuccess);
        var permissionId = created.Value;

        var assigned = await new AssignRolePermissionsCommandHandler(db)
            .Handle(new AssignRolePermissionsCommand(role.Id, [permissionId]), default);
        Assert.True(assigned.IsSuccess);

        var list = await new GetPermissionsQueryHandler(db).Handle(new GetPermissionsQuery(), default);
        Assert.Equal(1, list.Value!.Single(x => x.Id == permissionId).RoleCount);

        var updated = await new UpdatePermissionCommandHandler(db)
            .Handle(new UpdatePermissionCommand(permissionId, "quality:inspect", "执行质检"), default);
        Assert.True(updated.IsSuccess);

        var protectedResult = await new DeletePermissionCommandHandler(db)
            .Handle(new DeletePermissionCommand(adminPermission.Id), default);
        Assert.Equal(PermissionErrors.Protected.Code, protectedResult.Error.Code);

        var deleted = await new DeletePermissionCommandHandler(db)
            .Handle(new DeletePermissionCommand(permissionId), default);
        Assert.True(deleted.IsSuccess);
        Assert.False(await db.Permissions.AnyAsync(x => x.Id == permissionId));
        Assert.False(await db.RolePermissions.AnyAsync(x => x.PermissionId == permissionId));
    }
}
