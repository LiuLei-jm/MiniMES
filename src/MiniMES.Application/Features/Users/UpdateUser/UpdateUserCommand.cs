using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Users.UpdateUser;

public sealed record UpdateUserCommand(Guid UserId, string UserName, bool IsActive) : IRequest<Result>, IAuditableRequest
{
    public string AuditAction => "User.Update";
    public string AuditTargetType => "User";
    public string AuditTargetId => UserId.ToString();
}
