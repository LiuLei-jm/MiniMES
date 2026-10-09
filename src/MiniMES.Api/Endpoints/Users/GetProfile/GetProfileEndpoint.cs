using MiniMES.Api.Common.Extensions;
using MiniMES.Application.Features.Users.GetProfile;
using MiniMES.Application.Features.Users.Models;

namespace MiniMES.Api.Endpoints.Users.GetProfile;

public sealed class GetProfileEndpoint(ISender sender) : EndpointWithoutRequest<UserDetails>
{
    public override void Configure()
    {
        Get("/api/users/me");
        Summary(x => x.Summary = "查询当前用户资料");
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var result = await sender.Send(new GetProfileQuery(), ct);
        await Send.OkAsync(result.GetValue(), ct);
    }
}
