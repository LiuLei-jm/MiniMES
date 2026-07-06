using MiniMES.Application.Authentication;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Domain.Entities;

namespace MiniMES.Infrastructure.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IUserPermissionService _permissionService;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IApplicationDbContext _context;

    public AuthenticationService(IUserPermissionService permissionService, IJwtTokenGenerator jwtTokenGenerator, IRefreshTokenService refreshTokenService, IApplicationDbContext context)
    {
        _permissionService = permissionService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _refreshTokenService = refreshTokenService;
        _context = context;
    }

    public async Task<AuthenticationResult> IssueTokenPairAsync(User user, CancellationToken cancellationToken = default)
    {
        var permissionInfo = await _permissionService.GetPermissionsAsync(user.Id, cancellationToken);
        var jwtUser = new JwtUserInfo
        {
            UserId = user.Id,
            UserName = user.UserName,
            Roles = permissionInfo.Roles,
            Permissions = permissionInfo.Permissions
        };
        var jwt = _jwtTokenGenerator.Generate(jwtUser);
        var refreshToken = _refreshTokenService.Create(user.Id);
        _context.RefreshTokens.Add(refreshToken);
        return new AuthenticationResult
        {
            AccessToken = jwt.AccessToken,
            ExpiresAt = jwt.ExpiresAt,
            RefreshToken = refreshToken.Token
        };
    }


}
