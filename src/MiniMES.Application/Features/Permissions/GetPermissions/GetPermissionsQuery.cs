using MiniMES.Application.Common.Models;
using MiniMES.Application.Features.Permissions.Models;

namespace MiniMES.Application.Features.Permissions.GetPermissions;

public sealed record GetPermissionsQuery : IRequest<Result<IReadOnlyList<PermissionModel>>>;
