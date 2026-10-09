using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Application.Features.Users.Models;

namespace MiniMES.Application.Features.Users.GetProfile;

public sealed class GetProfileQueryHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<GetProfileQuery, Result<UserDetails>>
{
    public async Task<Result<UserDetails>> Handle(GetProfileQuery request, CancellationToken cancellationToken)
    {
        var user = await db.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == currentUser.UserId, cancellationToken);
        if (user is null) return Result<UserDetails>.Failure(UserErrors.UserNotFound);

        var roles = await (from userRole in db.UserRoles
                           join role in db.Roles on userRole.RoleId equals role.Id
                           where userRole.UserId == user.Id
                           select role.Name).Distinct().OrderBy(x => x).ToListAsync(cancellationToken);
        var permissions = await (from userRole in db.UserRoles
                                 join rolePermission in db.RolePermissions on userRole.RoleId equals rolePermission.RoleId
                                 join permission in db.Permissions on rolePermission.PermissionId equals permission.Id
                                 where userRole.UserId == user.Id
                                 select permission.Code).Distinct().OrderBy(x => x).ToListAsync(cancellationToken);
        return Result<UserDetails>.Success(new UserDetails(
            user.Id, user.UserName, user.IsActive, user.CreatedTime, user.LastLoginAt, roles, permissions));
    }
}
