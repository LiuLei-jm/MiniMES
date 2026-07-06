using MiniMES.Application.Authentication.Constrants;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Identity.Login;

public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<AuthenticationResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ILoginAttemptService _loginAttemptService;
    private readonly IAuthenticationService _tokenService;

    public LoginCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, ILoginAttemptService loginAttemptService, IAuthenticationService tokenService)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _loginAttemptService = loginAttemptService;
        _tokenService = tokenService;
    }

    public async Task<Result<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName, cancellationToken);
        if (user is null) return Result<AuthenticationResult>.Failure(AuthErrors.InvalidCredential);
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow) return Result<AuthenticationResult>.Failure(UserErrors.UserLocked);
        if (!user.IsActive) return Result<AuthenticationResult>.Failure(UserErrors.UserDisabled);
        if (_loginAttemptService.IsBlocked(request.UserName)) return Result<AuthenticationResult>.Failure(AuthErrors.TooManyAttempts);
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.AddFailedLoginAttempt();
            if (user.FailedLoginCount >= SecurityPolicy.MaxFailedAttempts)
            {
                user.ModifyLockoutEnd(DateTime.UtcNow.AddMinutes(SecurityPolicy.LockoutMinutes));
                user.ClearFailedLoginAttempts();
            }
            await _context.SaveChangesAsync(cancellationToken);
            _loginAttemptService.RecordFailure(request.UserName);
            return Result<AuthenticationResult>.Failure(AuthErrors.InvalidCredential);
        }
        user.LoginSucceeded();
        _loginAttemptService.Reset(request.UserName);
        var result = await _tokenService.IssueTokenPairAsync(user, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return Result<AuthenticationResult>.Success(result);
    }
}
