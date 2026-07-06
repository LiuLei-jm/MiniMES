using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Features.Identity.Login;

namespace MiniMES.Api.Endpoints.Identity.Login;

internal static class Mapper
{
    public static LoginCommand ToCommand(this LoginRequest request)
    {
        return new LoginCommand
        (
             request.UserName,
             request.Password
        );
    }
    public static LoginResponse ToResponse(this AuthenticationResult result)
    {
        return new LoginResponse
        {
            AccessToken = result.AccessToken,
            ExpiresAt = result.ExpiresAt,
            RefreshToken = result.RefreshToken
        };
    }
}
