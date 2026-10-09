using MiniMES.Api.Common.Extensions;
using MiniMES.Application.Authorization;
using MiniMES.Application.Features.Roles.CreateRole;
using MiniMES.Application.Features.Roles.DeleteRole;
using MiniMES.Application.Features.Roles.GetRoles;
using MiniMES.Application.Features.Roles.Models;
using MiniMES.Application.Features.Roles.UpdateRole;

namespace MiniMES.Api.Endpoints.Roles;

public sealed class RoleRouteRequest { public Guid Id { get; init; } }
public sealed class SaveRoleRequest { public Guid Id { get; init; } public string Name { get; init; } = string.Empty; }
public sealed record CreateRoleResponse(Guid Id, string Name);

[Permission("roles:view")]
public sealed class GetRolesEndpoint(ISender sender) : EndpointWithoutRequest<IReadOnlyList<RoleListItem>>
{
    public override void Configure() { Get("/api/roles"); Policies("permission:roles:view"); Summary(x => x.Summary = "查询角色"); }
    public override async Task HandleAsync(CancellationToken ct) => await Send.OkAsync((await sender.Send(new GetRolesQuery(), ct)).GetValue(), ct);
}

[Permission("roles:create")]
public sealed class CreateRoleEndpoint(ISender sender) : Endpoint<SaveRoleRequest, CreateRoleResponse>
{
    public override void Configure() { Post("/api/roles"); Policies("permission:roles:create"); Summary(x => x.Summary = "创建角色"); }
    public override async Task HandleAsync(SaveRoleRequest request, CancellationToken ct)
    {
        var id = (await sender.Send(new CreateRoleCommand(request.Name), ct)).GetValue();
        await Send.OkAsync(new CreateRoleResponse(id, request.Name.Trim()), ct);
    }
}

[Permission("roles:update")]
public sealed class UpdateRoleEndpoint(ISender sender) : Endpoint<SaveRoleRequest>
{
    public override void Configure() { Put("/api/roles/{id}"); Policies("permission:roles:update"); Summary(x => x.Summary = "更新角色"); }
    public override async Task HandleAsync(SaveRoleRequest request, CancellationToken ct)
    {
        (await sender.Send(new UpdateRoleCommand(request.Id, request.Name), ct)).ThrowIfFailure();
        await Send.NoContentAsync(ct);
    }
}

[Permission("roles:delete")]
public sealed class DeleteRoleEndpoint(ISender sender) : Endpoint<RoleRouteRequest>
{
    public override void Configure() { Delete("/api/roles/{id}"); Policies("permission:roles:delete"); Summary(x => x.Summary = "删除角色"); }
    public override async Task HandleAsync(RoleRouteRequest request, CancellationToken ct)
    {
        (await sender.Send(new DeleteRoleCommand(request.Id), ct)).ThrowIfFailure();
        await Send.NoContentAsync(ct);
    }
}
