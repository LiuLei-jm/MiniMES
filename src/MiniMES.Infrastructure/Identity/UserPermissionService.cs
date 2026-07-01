using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Interfaces.Security;
using MiniMES.Application.Common.Models;

namespace MiniMES.Infrastructure.Identity;

public sealed class UserPermissionService : IUserPermissionService
{
    private readonly IApplicationDbContext _context;

    public UserPermissionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserPermissionInfo> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var roles = await (from ur in _context.UserRoles
                           join r in _context.Roles on ur.RoleId equals r.Id
                           where ur.UserId == userId
                           select r.Name).Distinct().ToListAsync(cancellationToken);
        var permissions = await (from ur in _context.UserRoles
                                 join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
                                 join p in _context.Permissions on rp.PermissionId equals p.Id
                                 where ur.UserId == userId
                                 select p.Code).Distinct().ToListAsync(cancellationToken);
        return new UserPermissionInfo
        {
            Roles = roles,
            Permissions = permissions
        };
    }
}
