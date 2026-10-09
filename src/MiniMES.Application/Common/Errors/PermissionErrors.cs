using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Common.Errors;

public static class PermissionErrors
{
    public static readonly Error NotFound = new("Permission.NotFound", "权限不存在");
    public static readonly Error AlreadyExists = new("Permission.AlreadyExists", "权限编码已存在");
    public static readonly Error Protected = new("Permission.Protected", "system:admin 权限不能修改或删除");
}
