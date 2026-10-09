using MiniMES.Application.Authentication.Constrants;
using MiniMES.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiniMES.Infrastructure.Persistence.Seed;

public static class PermissionSeeder
{
    public static IEnumerable<Permission> Seed()
    {
        yield return new Permission(Permissions.Users.View, "查看用户");
        yield return new Permission(Permissions.Users.Create, "创建用户");
        yield return new Permission(Permissions.Users.Update, "修改用户");
        yield return new Permission(Permissions.Users.Delete, "删除用户");

        yield return new Permission(Permissions.Roles.View, "查看角色");
        yield return new Permission(Permissions.Roles.Create, "创建角色");
        yield return new Permission(Permissions.Roles.Update, "修改角色");
        yield return new Permission(Permissions.Roles.Delete, "删除角色");
        yield return new Permission(Permissions.Roles.Assign, "分配角色");

        yield return new Permission(Permissions.System.Admin, "系统管理员权限");
        yield return new Permission(Permissions.PermissionManagement.View, "查看权限");
        yield return new Permission(Permissions.PermissionManagement.Create, "创建权限");
        yield return new Permission(Permissions.PermissionManagement.Update, "修改权限");
        yield return new Permission(Permissions.PermissionManagement.Delete, "删除权限");
        yield return new Permission(Permissions.PermissionManagement.Assign, "分配权限");
        yield return new Permission(Permissions.Audit.View, "查看审计日志");
    }
}
