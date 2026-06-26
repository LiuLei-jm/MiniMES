namespace MiniMES.Api.Endpoints.Users.CreateUser;

public sealed class Response
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = default!;
}
