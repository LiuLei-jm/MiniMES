using MiniMES.Api.Common.Extensions;
using MiniMES.Application.Authorization;
using MiniMES.Application.Features.Roles.AssignUserRoles;

namespace MiniMES.Api.Endpoints.Users.AssignRoles;

public sealed class AssignUserRolesRequest
{
    public Guid Id { get; init; }
    public IReadOnlyList<Guid> RoleIds { get; init; } = [];
}

[Permission("roles:assign")]
public sealed class AssignUserRolesEndpoint(ISender sender) : Endpoint<AssignUserRolesRequest>
{
    public override void Configure() { Put("/api/users/{id}/roles"); Policies("permission:roles:assign"); Summary(x => x.Summary = "分配用户角色"); }
    public override async Task HandleAsync(AssignUserRolesRequest request, CancellationToken ct)
    {
        (await sender.Send(new AssignUserRolesCommand(request.Id, request.RoleIds), ct)).ThrowIfFailure();
        await Send.NoContentAsync(ct);
    }
}
