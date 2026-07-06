using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Common.Errors;

public static class TokenErrors
{
    public static readonly Error InvalidRefreshToken =
    new("Token.InvalidRefreshToken", "Refresh token 无效");
    public static readonly Error RefreshTokenExpired =
        new Error("Token.RefreshExpired", "Refresh token 已过期");
    public static readonly Error RefreshTokenRevoked =
        new Error("Token.RefreshRevoked", "Refresh token 已撤销");
}
