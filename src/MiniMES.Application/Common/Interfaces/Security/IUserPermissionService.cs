using MiniMES.Application.Common.Models;

namespace MiniMES.Application.Common.Interfaces.Security;

public interface IUserPermissionService
{
    Task<UserPermissionInfo> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
