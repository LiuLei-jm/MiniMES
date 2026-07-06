using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Common.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredential = new Error("Auth.InvalidCredential", "用户名或密码错误");
    public static readonly Error TokenExpired = new Error("Auth.TokenExpired", "Token 已过期");
    public static readonly Error InvalidRefreshToken = new Error("Auth.InvalidRefreshToken", "RefreshToken 无效");
    public static readonly Error ExpiredRefreshToken = new Error("Auth.ExpiredRefreshToken", "RefreshToken 已过期");
    public static readonly Error TooManyAttempts = new Error("Auth.TooManyAttempts", "尝试次数过多，请稍后再试");
}
