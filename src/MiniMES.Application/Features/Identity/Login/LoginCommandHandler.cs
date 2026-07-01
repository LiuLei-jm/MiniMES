using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Interfaces.Security;
using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Security;

namespace MiniMES.Application.Features.Identity.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _jwtTokenGenerator;
    private readonly IUserPermissionService _permissionService;

    public LoginCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IUserPermissionService permissionService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
        _permissionService = permissionService;
    }

    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.AsNoTracking()
                                       .FirstOrDefaultAsync(x => x.UserName == request.UserName, cancellationToken);
        if (user is null) return Result<LoginResult>.Failure(AuthErrors.InvalidCredential);
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash)) return Result<LoginResult>.Failure(AuthErrors.InvalidCredential);
        if (!user.IsActive) return Result<LoginResult>.Failure(UserErrors.UserDisabled);
        var permissionInfo = await _permissionService.GetPermissionsAsync(user.Id, cancellationToken);
        var jwtUser = new JwtUserInfo
        {
            UserId = user.Id,
            UserName = user.UserName,
            Roles = permissionInfo.Roles,
            Permissions = permissionInfo.Permissions
        };
        var accessToken = _jwtTokenGenerator.Generate(jwtUser);
        var expireAt = _jwtTokenGenerator.GetExpireTime();
        return Result<LoginResult>.Success(new LoginResult
        {
            AccessToken = accessToken,
            ExpiresAt = expireAt
        });
    }
}
