using MiniMES.Api.Common.Extensions;
using MiniMES.Application.Authorization;
using MiniMES.Application.Features.Permissions.CreatePermission;
using MiniMES.Application.Features.Permissions.DeletePermission;
using MiniMES.Application.Features.Permissions.GetPermissions;
using MiniMES.Application.Features.Permissions.Models;
using MiniMES.Application.Features.Permissions.UpdatePermission;

namespace MiniMES.Api.Endpoints.Permissions;

public sealed class PermissionRouteRequest { public Guid Id { get; init; } }
public sealed class SavePermissionRequest { public Guid Id { get; init; } public string Code { get; init; } = string.Empty; public string Description { get; init; } = string.Empty; }
public sealed record CreatePermissionResponse(Guid Id, string Code, string Description);

[Permission("permissions:view")]
public sealed class GetPermissionsEndpoint(ISender sender) : EndpointWithoutRequest<IReadOnlyList<PermissionModel>>
{
    public override void Configure() { Get("/api/permissions"); Policies("permission:permissions:view"); Summary(x => x.Summary = "查询权限"); }
    public override async Task HandleAsync(CancellationToken ct) => await Send.OkAsync((await sender.Send(new GetPermissionsQuery(), ct)).GetValue(), ct);
}

[Permission("permissions:create")]
public sealed class CreatePermissionEndpoint(ISender sender) : Endpoint<SavePermissionRequest, CreatePermissionResponse>
{
    public override void Configure() { Post("/api/permissions"); Policies("permission:permissions:create"); Summary(x => x.Summary = "创建权限"); }
    public override async Task HandleAsync(SavePermissionRequest request, CancellationToken ct)
    {
        var id = (await sender.Send(new CreatePermissionCommand(request.Code, request.Description), ct)).GetValue();
        await Send.OkAsync(new CreatePermissionResponse(id, request.Code.Trim().ToLowerInvariant(), request.Description.Trim()), ct);
    }
}

[Permission("permissions:update")]
public sealed class UpdatePermissionEndpoint(ISender sender) : Endpoint<SavePermissionRequest>
{
    public override void Configure() { Put("/api/permissions/{id}"); Policies("permission:permissions:update"); Summary(x => x.Summary = "更新权限"); }
    public override async Task HandleAsync(SavePermissionRequest request, CancellationToken ct)
    {
        (await sender.Send(new UpdatePermissionCommand(request.Id, request.Code, request.Description), ct)).ThrowIfFailure();
        await Send.NoContentAsync(ct);
    }
}

[Permission("permissions:delete")]
public sealed class DeletePermissionEndpoint(ISender sender) : Endpoint<PermissionRouteRequest>
{
    public override void Configure() { Delete("/api/permissions/{id}"); Policies("permission:permissions:delete"); Summary(x => x.Summary = "删除权限"); }
    public override async Task HandleAsync(PermissionRouteRequest request, CancellationToken ct)
    {
        (await sender.Send(new DeletePermissionCommand(request.Id), ct)).ThrowIfFailure();
        await Send.NoContentAsync(ct);
    }
}
