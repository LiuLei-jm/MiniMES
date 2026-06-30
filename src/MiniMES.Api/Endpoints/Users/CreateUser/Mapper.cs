using Mapster;
using MiniMES.Application.Features.Users.CreateUser;

namespace MiniMES.Api.Endpoints.Users.CreateUser;

public static class Mapper
{
    public static CreateUserCommand ToCommand(this CreateUserRequest request)
    {
        return request.Adapt<CreateUserCommand>();
    }
}
