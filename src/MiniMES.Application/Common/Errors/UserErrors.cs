using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Common.Errors;

public static class UserErrors
{
    public static readonly Error UserAlreadyExists = new Error("User.AlreadyExists", "用户名已存在");
    public static readonly Error UserNotFound = new Error("User.NotFound", "用户不存在");
    public static readonly Error UserDisabled = new Error("User.Disabled", "用户已被禁用");
    public static readonly Error UserLocked = new Error("User.Locked", "用户已被锁定，请稍后再试");
    public static readonly Error CannotDisableSelf = new Error("User.CannotDisableSelf", "不能停用当前登录账号");
}
