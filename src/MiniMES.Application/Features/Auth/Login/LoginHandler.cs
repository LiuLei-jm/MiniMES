using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Security;

namespace MiniMES.Application.Features.Auth.Login;

public class LoginHandler : IRequestHandler<LoginCommand, Result<string>>
{
    private readonly IApplicationDbContext _db;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IApplicationDbContext _context;
    private readonly IJwtTokenGenerator _jwt;
    public LoginHandler(IPasswordHasher passwordHasher, IJwtTokenGenerator jwt, IApplicationDbContext context, IApplicationDbContext db)
    {
        _passwordHasher = passwordHasher;
        _jwt = jwt;
        _context = context;
        _db = db;
    }

    public async Task<Result<string>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == request.UserName);
        if (user == null) return Result<string>.Failure(AuthErrors.UserNotFound);
        if (!_passwordHasher.Verify(request.Password, user.PasswordHash))
            return Result<string>.Failure(AuthErrors.InvalidPassword);
        var roles = await _context.UserRoles.Where(x => x.UserId == user.Id)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .ToListAsync();
        var roleIds = await _context.UserRoles.Where(x => x.UserId == user.Id)
            .Select(x => x.RoleId).ToListAsync();
        var permissions = await _context.RolePermissions
            .Where(x => roleIds.Contains(x.RoleId))
            .Join(_context.Permissions,
            rp => rp.PermissionId,
            p => p.Id,
            (rp, p) => p.Code)
            .Distinct()
            .ToListAsync();
        var token = await _jwt.GenerateAsync(user, roles, permissions);
        return Result<string>.Success(token);
    }
}
