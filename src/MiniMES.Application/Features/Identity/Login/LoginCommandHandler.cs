using MiniMES.Application.Authentication.Constrants;
using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Logging;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Identity.Login;

public sealed class LoginCommandHandler(IApplicationDbContext context, IPasswordHasher passwordHasher, ILoginAttemptService loginAttempts, IAuthenticationService tokenService, ICurrentRequestContext requestContext, ISecurityAuditService auditService) : IRequestHandler<LoginCommand, Result<AuthenticationResult>>
{
    public async Task<Result<AuthenticationResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        async Task<Result<AuthenticationResult>> Failure(Error error)
        {
            await auditService.LogAsync(new AuditEvent("Login", "Login", false, request.UserName, Details: error.Code), cancellationToken);
            return Result<AuthenticationResult>.Failure(error);
        }

        var ip = requestContext.ClientIp;
        if (await loginAttempts.IsBlockedAsync(request.UserName, ip, cancellationToken)) return await Failure(AuthErrors.TooManyAttempts);
        var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == request.UserName, cancellationToken);
        if (user is null)
        {
            await loginAttempts.RecordFailureAsync(request.UserName, ip, cancellationToken);
            return await Failure(AuthErrors.InvalidCredential);
        }
        if (user.LockoutEnd.HasValue && user.LockoutEnd > DateTime.UtcNow) return await Failure(UserErrors.UserLocked);
        if (!user.IsActive) return await Failure(UserErrors.UserDisabled);
        if (!passwordHasher.Verify(request.Password, user.PasswordHash))
        {
            user.AddFailedLoginAttempt();
            if (user.FailedLoginCount >= SecurityPolicy.MaxFailedAttempts) { user.ModifyLockoutEnd(DateTime.UtcNow.AddMinutes(SecurityPolicy.LockoutMinutes)); user.ClearFailedLoginAttempts(); }
            await context.SaveChangesAsync(cancellationToken);
            await loginAttempts.RecordFailureAsync(request.UserName, ip, cancellationToken);
            return await Failure(AuthErrors.InvalidCredential);
        }
        user.LoginSucceeded();
        await loginAttempts.ResetAsync(request.UserName, ip, cancellationToken);
        var result = await tokenService.IssueTokenPairAsync(user, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        await auditService.LogAsync(new AuditEvent("Login", "Login", true, request.UserName, TargetType: "User", TargetId: user.Id.ToString()), cancellationToken);
        return Result<AuthenticationResult>.Success(result);
    }
}
