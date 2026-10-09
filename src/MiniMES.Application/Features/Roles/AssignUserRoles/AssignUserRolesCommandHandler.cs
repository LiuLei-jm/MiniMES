using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;
using MiniMES.Domain.Entities;

namespace MiniMES.Application.Features.Roles.AssignUserRoles;

public sealed class AssignUserRolesCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<AssignUserRolesCommand, Result>
{
    public async Task<Result> Handle(AssignUserRolesCommand request, CancellationToken cancellationToken)
    {
        if (request.UserId == currentUser.UserId) return Result.Failure(RoleErrors.CannotModifyOwnRoles);
        if (!await db.Users.AnyAsync(x => x.Id == request.UserId, cancellationToken)) return Result.Failure(UserErrors.UserNotFound);
        var roleIds = request.RoleIds.Distinct().ToList();
        if (await db.Roles.CountAsync(x => roleIds.Contains(x.Id), cancellationToken) != roleIds.Count) return Result.Failure(RoleErrors.NotFound);

        db.UserRoles.RemoveRange(db.UserRoles.Where(x => x.UserId == request.UserId));
        db.UserRoles.AddRange(roleIds.Select(roleId => new UserRole { UserId = request.UserId, RoleId = roleId }));
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
