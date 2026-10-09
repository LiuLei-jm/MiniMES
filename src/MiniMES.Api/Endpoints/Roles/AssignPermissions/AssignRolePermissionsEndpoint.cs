using MiniMES.Api.Common.Extensions;
using MiniMES.Application.Authorization;
using MiniMES.Application.Features.Permissions.AssignRolePermissions;

namespace MiniMES.Api.Endpoints.Roles.AssignPermissions;

public sealed class AssignRolePermissionsRequest
{
    public Guid Id { get; init; }
    public IReadOnlyList<Guid> PermissionIds { get; init; } = [];
}

[Permission("permissions:assign")]
public sealed class AssignRolePermissionsEndpoint(ISender sender) : Endpoint<AssignRolePermissionsRequest>
{
    public override void Configure() { Put("/api/roles/{id}/permissions"); Policies("permission:permissions:assign"); Summary(x => x.Summary = "分配角色权限"); }
    public override async Task HandleAsync(AssignRolePermissionsRequest request, CancellationToken ct)
    {
        (await sender.Send(new AssignRolePermissionsCommand(request.Id, request.PermissionIds), ct)).ThrowIfFailure();
        await Send.NoContentAsync(ct);
    }
}
