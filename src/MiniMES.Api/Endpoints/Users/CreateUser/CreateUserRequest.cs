namespace MiniMES.Api.Endpoints.Users.CreateUser;

public sealed class CreateUserRequest
{
    public string UserName { get; init; } = default!;
    public string Password { get; init; } = default!;
}
