using MiniMES.Api.Common.Extensions;
using MiniMES.Application.Authorization;
using MiniMES.Application.Features.Users.UpdateUser;

namespace MiniMES.Api.Endpoints.Users.UpdateUser;

public sealed class UpdateUserRequest
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = string.Empty;
    public bool IsActive { get; init; }
}

[Permission("users:update")]
public sealed class UpdateUserEndpoint(ISender sender) : Endpoint<UpdateUserRequest>
{
    public override void Configure()
    {
        Put("/api/users/{id}");
        Policies("permission:users:update");
        Summary(x => x.Summary = "更新用户");
    }

    public override async Task HandleAsync(UpdateUserRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new UpdateUserCommand(request.Id, request.UserName, request.IsActive), ct);
        result.ThrowIfFailure();
        await Send.NoContentAsync(ct);
    }
}
