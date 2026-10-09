using MiniMES.Application.Common.Models;
using MiniMES.Application.Common.Interfaces;

namespace MiniMES.Application.Features.Users.CreateUser;

public sealed record CreateUserCommand(string UserName, string Password) : IRequest<Result<Guid>>, IAuditableRequest
{
    public string AuditAction => "User.Create";
    public string AuditTargetType => "User";
    public string? AuditTargetId => UserName;
}
