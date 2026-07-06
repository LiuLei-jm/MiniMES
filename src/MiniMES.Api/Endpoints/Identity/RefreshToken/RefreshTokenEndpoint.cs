using MiniMES.Api.Common.Extensions;
using MiniMES.Api.Endpoints.Identity.Login;

namespace MiniMES.Api.Endpoints.Identity.RefreshToken;

public sealed class RefreshTokenEndpoint : Endpoint<RefreshTokenRequest, LoginResponse>
{
    private readonly ISender _sender;

    public RefreshTokenEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("/api/auth/refresh");
        AllowAnonymous();
    }
    public override async Task HandleAsync(
        RefreshTokenRequest req,
        CancellationToken ct)
    {
        var result = await _sender.Send(req.ToCommand(), ct);
        result.ThrowIfFailure();
        await Send.OkAsync(result.GetValue().ToResponse(), ct);
    }
}
