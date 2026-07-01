using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Common.Errors;

public static class AuthErrors
{
    public static readonly Error InvalidCredential = new Error("Auth.InvalidCredential", "用户名或密码错误");
    public static readonly Error TokenExpired = new Error("Auth.TokenExpired", "Token 已过期");
    public static readonly Error RefreshTokenInvalid = new Error("Auth.RefreshTokenInvalid", "RefreshToken 无效");
}
