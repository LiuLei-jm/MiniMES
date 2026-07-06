using MiniMES.Api.Endpoints.Identity.Login;
using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Features.Identity.Refresh;

namespace MiniMES.Api.Endpoints.Identity.RefreshToken;

internal static class Mapper
{
    public static RefreshTokenCommand ToCommand(this RefreshTokenRequest request)
    {
        return new RefreshTokenCommand(request.RefreshToken);
    }
    public static LoginResponse ToResponse(this AuthenticationResult result)
    {
        return new LoginResponse
        {
            AccessToken = result.AccessToken,
            ExpiresAt = result.ExpiresAt,
            RefreshToken = result.RefreshToken,
        };
    }
}
