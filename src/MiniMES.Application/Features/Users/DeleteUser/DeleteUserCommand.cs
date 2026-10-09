using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Users.DeleteUser;

public sealed record DeleteUserCommand(Guid UserId) : IRequest<Result>, IAuditableRequest
{
    public string AuditAction => "User.Disable";
    public string AuditTargetType => "User";
    public string AuditTargetId => UserId.ToString();
}
