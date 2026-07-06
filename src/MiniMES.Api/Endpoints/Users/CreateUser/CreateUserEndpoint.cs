using MiniMES.Application.Authorization;

namespace MiniMES.Api.Endpoints.Users.CreateUser;

[Permission("users:create")]
public sealed class CreateUserEndpoint : Endpoint<CreateUserRequest, CreateUserResponse>
{
    private readonly ISender _sender;

    public CreateUserEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("api/users");
        Policies("permission:users:create");
        Summary(s =>
        {
            s.Summary = "创建用户";
            s.Description = "创建一个新用户";
            s.Response(200, "创建成功");
        });
    }
    public override async Task HandleAsync(CreateUserRequest req, CancellationToken ct)
    {
        var result = await _sender.Send(req.ToCommand(), ct);
        if (result.IsFailure)
        {
            throw new Exception(result.Error.Message);
        }
        await Send.OkAsync(new CreateUserResponse
        {
            Id = result.Value!,
            UserName = req.UserName
        }, ct);
    }
}
