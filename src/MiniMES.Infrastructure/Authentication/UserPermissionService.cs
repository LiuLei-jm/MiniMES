using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Authentication.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Infrastructure.Authentication;

public sealed class UserPermissionService : IUserPermissionService
{
    private readonly IApplicationDbContext _context;

    public UserPermissionService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<UserPermissionInfo> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var roleNames = await (from ur in _context.UserRoles
                               join r in _context.Roles on ur.RoleId equals r.Id
                               where ur.UserId == userId
                               select r.Name).Distinct().ToListAsync(cancellationToken);
        var permissionCodes = await (from ur in _context.UserRoles
                                     join rp in _context.RolePermissions on ur.RoleId equals rp.RoleId
                                     join p in _context.Permissions on rp.PermissionId equals p.Id
                                     where ur.UserId == userId
                                     select p.Code).Distinct().ToListAsync(cancellationToken);
        return new UserPermissionInfo
        {
            Roles = roleNames,
            Permissions = permissionCodes
        };
    }
}
