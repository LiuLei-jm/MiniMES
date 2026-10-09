using MiniMES.Api.Common.Extensions;
using MiniMES.Application.Authorization;
using MiniMES.Application.Features.Users.GetUser;
using MiniMES.Application.Features.Users.Models;

namespace MiniMES.Api.Endpoints.Users.GetUser;

public sealed class GetUserRequest { public Guid Id { get; init; } }

[Permission("users:view")]
public sealed class GetUserEndpoint(ISender sender) : Endpoint<GetUserRequest, UserDetails>
{
    public override void Configure()
    {
        Get("/api/users/{id}");
        Policies("permission:users:view");
        Summary(x => x.Summary = "查询用户详情");
    }

    public override async Task HandleAsync(GetUserRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserQuery(request.Id), ct);
        await Send.OkAsync(result.GetValue(), ct);
    }
}
