using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Logging;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Identity.Refresh;

public sealed class RefreshTokenCommnadHandler(IApplicationDbContext context, IAuthenticationService tokenService, IRefreshTokenService refreshTokenService, ISecurityAuditService auditService) : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResult>>
{
    public async Task<Result<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        async Task<Result<AuthenticationResult>> Failure(Error error)
        {
            await auditService.LogAsync(new AuditEvent("Security", "Token.Refresh", false, Details: error.Code), cancellationToken);
            return Result<AuthenticationResult>.Failure(error);
        }

        var oldToken = await context.RefreshTokens.FirstOrDefaultAsync(x => x.TokenHash == refreshTokenService.Hash(request.RefreshToken), cancellationToken);
        if (oldToken is null) return await Failure(TokenErrors.InvalidRefreshToken);
        if (oldToken.IsExpired) return await Failure(TokenErrors.RefreshTokenExpired);
        if (oldToken.IsRevoked) return await Failure(TokenErrors.RefreshTokenRevoked);

        var user = await context.Users.FirstOrDefaultAsync(u => u.Id == oldToken.UserId, cancellationToken);
        if (user is null) return await Failure(UserErrors.UserNotFound);
        if (!user.IsActive) return await Failure(UserErrors.UserDisabled);
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow) return await Failure(UserErrors.UserLocked);

        var result = await tokenService.IssueTokenPairAsync(user, cancellationToken);
        oldToken.Revoke(refreshTokenService.Hash(result.RefreshToken));
        await context.SaveChangesAsync(cancellationToken);
        await auditService.LogAsync(new AuditEvent("Security", "Token.Refresh", true, user.UserName, "User", user.Id.ToString()), cancellationToken);
        return Result<AuthenticationResult>.Success(result);
    }
}
