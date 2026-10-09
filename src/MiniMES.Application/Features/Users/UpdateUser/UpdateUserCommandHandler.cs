using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Users.UpdateUser;

public sealed class UpdateUserCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user is null) return Result.Failure(UserErrors.UserNotFound);
        if (!request.IsActive && request.UserId == currentUser.UserId) return Result.Failure(UserErrors.CannotDisableSelf);
        if (await db.Users.AnyAsync(x => x.Id != request.UserId && x.UserName == request.UserName, cancellationToken))
            return Result.Failure(UserErrors.UserAlreadyExists);

        user.Update(request.UserName.Trim(), request.IsActive);
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
