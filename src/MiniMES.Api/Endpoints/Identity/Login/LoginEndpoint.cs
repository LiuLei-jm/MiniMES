using MiniMES.Api.Common.Extensions;

namespace MiniMES.Api.Endpoints.Identity.Login;

public sealed class LoginEndpoint : Endpoint<LoginRequest, LoginResponse>
{
    private readonly ISender _sender;

    public LoginEndpoint(ISender sender)
    {
        _sender = sender;
    }
    public override void Configure()
    {
        Post("/api/auth/login");
        AllowAnonymous();
        Summary(s =>
        {
            s.Summary = "用户登录";
            s.Description = "用户名密码登录";
            s.Response<LoginResponse>(200, "登录成功");
        });
    }
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var result = await _sender.Send(req.ToCommand(), ct);
        result.ThrowIfFailure();
        await Send.OkAsync(result.GetValue().ToResponse(), ct);
    }
}
