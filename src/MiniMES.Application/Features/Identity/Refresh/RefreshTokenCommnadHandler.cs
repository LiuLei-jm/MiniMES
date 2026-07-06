using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Identity.Refresh;

public class RefreshTokenCommnadHandler : IRequestHandler<RefreshTokenCommand, Result<AuthenticationResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuthenticationService _tokenService;

    public RefreshTokenCommnadHandler(IApplicationDbContext context, IAuthenticationService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthenticationResult>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var oldToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == request.RefreshToken, cancellationToken);

        if (oldToken is null)
        {
            return Result<AuthenticationResult>.Failure(TokenErrors.InvalidRefreshToken);
        }

        if (oldToken.IsExpired)
        {
            return Result<AuthenticationResult>.Failure(AuthErrors.TokenExpired);
        }
        if (oldToken.IsRevoked) return Result<AuthenticationResult>.Failure(TokenErrors.RefreshTokenRevoked);

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == oldToken.UserId, cancellationToken);
        if (user is null) return Result<AuthenticationResult>.Failure(UserErrors.UserNotFound);
        var result = await _tokenService.IssueTokenPairAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<AuthenticationResult>.Success(result);
    }
}
