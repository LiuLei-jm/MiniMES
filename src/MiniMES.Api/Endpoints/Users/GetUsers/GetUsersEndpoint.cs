using MiniMES.Api.Common.Extensions;
using MiniMES.Application.Authorization;
using MiniMES.Application.Features.Users.GetUsers;
using MiniMES.Application.Features.Users.Models;

namespace MiniMES.Api.Endpoints.Users.GetUsers;

public sealed class GetUsersRequest
{
    public string? Keyword { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 20;
}

[Permission("users:view")]
public sealed class GetUsersEndpoint(ISender sender) : Endpoint<GetUsersRequest, UserPage>
{
    public override void Configure()
    {
        Get("/api/users");
        Policies("permission:users:view");
        Summary(x => x.Summary = "分页查询用户");
    }

    public override async Task HandleAsync(GetUsersRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new GetUsersQuery(request.Keyword, request.Page, request.PageSize), ct);
        await Send.OkAsync(result.GetValue(), ct);
    }
}
