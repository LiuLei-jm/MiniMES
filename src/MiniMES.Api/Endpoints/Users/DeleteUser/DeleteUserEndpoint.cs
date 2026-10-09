using MiniMES.Api.Common.Extensions;
using MiniMES.Application.Authorization;
using MiniMES.Application.Features.Users.DeleteUser;

namespace MiniMES.Api.Endpoints.Users.DeleteUser;

public sealed class DeleteUserRequest { public Guid Id { get; init; } }

[Permission("users:delete")]
public sealed class DeleteUserEndpoint(ISender sender) : Endpoint<DeleteUserRequest>
{
    public override void Configure()
    {
        Delete("/api/users/{id}");
        Policies("permission:users:delete");
        Summary(x => x.Summary = "停用用户");
    }

    public override async Task HandleAsync(DeleteUserRequest request, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteUserCommand(request.Id), ct);
        result.ThrowIfFailure();
        await Send.NoContentAsync(ct);
    }
}
