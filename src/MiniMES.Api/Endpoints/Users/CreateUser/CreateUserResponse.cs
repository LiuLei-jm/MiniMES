namespace MiniMES.Api.Endpoints.Users.CreateUser;

public sealed class CreateUserResponse
{
    public Guid Id { get; init; }
    public string UserName { get; init; } = default!;
}
