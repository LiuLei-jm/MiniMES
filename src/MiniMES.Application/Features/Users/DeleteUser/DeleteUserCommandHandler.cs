using MiniMES.Application.Authentication.Interfaces;
using MiniMES.Application.Common.Errors;
using MiniMES.Application.Common.Interfaces;
using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Features.Users.DeleteUser;

public sealed class DeleteUserCommandHandler(IApplicationDbContext db, ICurrentUser currentUser)
    : IRequestHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await db.Users.SingleOrDefaultAsync(x => x.Id == request.UserId, cancellationToken);
        if (user is null) return Result.Failure(UserErrors.UserNotFound);
        if (request.UserId == currentUser.UserId) return Result.Failure(UserErrors.CannotDisableSelf);

        user.Disable();
        await db.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
