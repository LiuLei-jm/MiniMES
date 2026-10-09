using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Authentication;

public sealed class AuthenticationService(IUserPermissionService permissionService, IJwtTokenGenerator jwtTokenGenerator, IRefreshTokenService refreshTokenService, IApplicationDbContext context) : IAuthenticationService
{
    public async Task<AuthenticationResult> IssueTokenPairAsync(User user, CancellationToken cancellationToken = default)
    {
        var permissionInfo = await permissionService.GetPermissionsAsync(user.Id, cancellationToken);
        var jwt = jwtTokenGenerator.Generate(new JwtUserInfo { UserId = user.Id, UserName = user.UserName, Roles = permissionInfo.Roles, Permissions = permissionInfo.Permissions });
        var refresh = refreshTokenService.Create(user.Id);
        context.RefreshTokens.Add(refresh.Entity);
        return new AuthenticationResult { AccessToken = jwt.AccessToken, ExpiresAt = jwt.ExpiresAt, RefreshToken = refresh.Token };
    }
}
