using MediatR;

namespace MiniMES.Api.Endpoints.Users.CreateUser;

public sealed class CreateUserEndpoint : Endpoint<Request, Response>
{
    private readonly ISender _sender;

    public CreateUserEndpoint(ISender sender)
    {
        _sender = sender;
    }

    public override void Configure()
    {
        Post("/users");
        //Policies("Users.Create");
        AllowAnonymous();
    }
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var result = await _sender.Send(req.ToCommand(), ct);
        if (result.IsFailure)
        {
            throw new Exception(result.Error.Message);
        }
        await Send.OkAsync(new Response
        {
            Id = result.Value!,
            UserName = req.UserName
        }, ct);
    }
}
