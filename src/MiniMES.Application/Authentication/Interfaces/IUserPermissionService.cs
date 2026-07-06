using MiniMES.Application.Authentication.Models;

namespace MiniMES.Application.Authentication.Interfaces;

public interface IUserPermissionService
{
    Task<UserPermissionInfo> GetPermissionsAsync(Guid userId, CancellationToken cancellationToken = default);
}
