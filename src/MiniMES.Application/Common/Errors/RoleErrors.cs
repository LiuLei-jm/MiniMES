using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Common.Errors;

public static class RoleErrors
{
    public static readonly Error NotFound = new("Role.NotFound", "角色不存在");
    public static readonly Error AlreadyExists = new("Role.AlreadyExists", "角色名称已存在");
    public static readonly Error BuiltInRole = new("Role.BuiltIn", "内置 Admin 角色不能修改或删除");
    public static readonly Error CannotModifyOwnRoles = new("Role.CannotModifyOwnRoles", "不能修改当前登录账号的角色");
}
