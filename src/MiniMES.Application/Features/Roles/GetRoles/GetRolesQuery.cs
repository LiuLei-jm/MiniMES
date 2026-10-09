using MiniMES.Application.Common.Models;
using MiniMES.Application.Features.Roles.Models;

namespace MiniMES.Application.Features.Roles.GetRoles;

public sealed record GetRolesQuery : IRequest<Result<IReadOnlyList<RoleListItem>>>;
